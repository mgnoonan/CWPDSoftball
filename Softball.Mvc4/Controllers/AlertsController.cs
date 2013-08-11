using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using HtmlAgilityPack;
using Quiksoft.FreeSMTP;

using Softball.Mvc4.Helpers;
using Softball.Mvc4.Models;
using Softball.Mvc4.Utilities;

namespace Softball.Mvc4.Controllers
{
    [Authorize]
    public class AlertsController : _Controller
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public AlertsController() : this(new Repository()) { }
        public AlertsController(IRepository repository) { _repository = repository; }
        
        //
        // GET: /Alerts/

        public ActionResult Index()
        {
            var playerList = _repository.FindPlayers()
                                        .OrderByDescending(p => p.IsActive).ThenBy(p => p.LastName)
                                        .Where(g => !g.LastName.Contains("Noonan"))
                                        .ToList();

            var checkList = (from p in playerList
                             select new CheckBoxListInfo
                             {
                                 DisplayText = p.LastName + ", " + p.FirstName + " " + p.CellNumber,
                                 Value = p.PlayerID.ToString(),
                                 IsChecked = p.IsActive
                             }).ToList();

            return View(checkList);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendAlerts(bool chkUpdateFieldStatus, bool chkIncludeFieldStatus, bool chkUseGoogleVoice, string message, string code, string[] playerList)
        {
            if (chkUpdateFieldStatus)
                DoUpdateFieldStatus();

            // Add selected recipients
            var allPlayers = _repository.FindPlayers().ToList();
            var players = (from p in allPlayers where playerList.Contains(p.PlayerID.ToString()) select p);

            if (players.Count() > 0)
            {
                SendEmail(chkIncludeFieldStatus, !chkUseGoogleVoice, message, players);
                if (chkUseGoogleVoice)
                {
                    SendSMS(chkIncludeFieldStatus, message, players.Where(p => p.IsTextEnabled || p.PlayerID == 1));
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Update()
        {
            DoUpdateFieldStatus();

            return RedirectToAction("Index", "Home");
        }

        private void SendSMS(bool includeFieldStatus, string extraMessage, IEnumerable<Player> players)
        {
            //
            // Login to Google Voice
            //

            //var voice = new Google.Voice.GoogleVoiceClient();
            //voice.Login("mgnoonan.public@gmail.com", "");
            ////voice.SendSMS("9376091518,9376095905", getTextMessageBody(chkIncludeFieldStatus, message).Left(140));

            //if (!voice.LoggedIn)
            //    return;
            var voice = new Jitbit.Utils.SharpGoogleVoice("", "");


            //
            // Initialize working variables 
            //

            int count = 0;
            StringBuilder sb = new StringBuilder();
            var message = getTextMessageBody(includeFieldStatus, extraMessage).Left(160);


            //
            // Send the messsage via Google Voice SMS service, up to 5 numbers at a time
            //

            foreach (Player p in players)
            {
                try
                {
                    //// Add the next number to the list
                    //if (p.IsTextEnabled && p.TextEmail.Length > 0)
                    //{
                    //    sb.AppendFormat("+1{0},", p.TextEmail.Left(10));
                    //    count++;
                    //}

                    //// Send the SMS when we hit the 5th phone number
                    //if (count % 5 == 0 && !string.IsNullOrWhiteSpace(sb.ToString()))
                    //{
                    //    // Strip off the trailing comma
                    //    sb.Length--;
                    //    // Send the text message
                    //    voice.SendSMS(sb.ToString(), message);
                    //    // Clear out the list
                    //    sb.Clear();
                    //    // Pause for 5 seconds between calls (Google apparently throttles after 5 SMS messages)
                    //    //System.Threading.Thread.Sleep(5000);
                    //}
                    string number = "+1" + p.TextEmail.Left(10);
                    voice.SendSMS(number, message);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }


            //
            // Send SMS to any leftover numbers from the loop
            //

            //if (!string.IsNullOrWhiteSpace(sb.ToString()))
            //{
            //    // Strip off the trailing comma
            //    sb.Length--;
            //    // Send the text message
            //    voice.SendSMS(sb.ToString(), message);
            //}

            //voice.Logout();
            //voice = null;
        }

        private void SendEmail(bool includeFieldStatus, bool includeSmsEmails, string extraMessage, IEnumerable<Player> players)
        {
            string teamName = _repository.GetTeamName();

            EmailMessage Message = new EmailMessage();
            Message.From.Email = "";
            Message.From.Name = teamName + " Update";
            Message.Subject = teamName + " Update";

            foreach (Player p in players)
            {
                if (p.PlayerID == 1)
                    Message.Recipients.Add(p.Email, p.FirstName + " " + p.LastName, RecipientType.To);

                if (p.IsEmailEnabled && p.Email.Length > 0)
                    Message.Recipients.Add(p.Email, p.FirstName + " " + p.LastName, RecipientType.BCC);

                if (includeSmsEmails)
                {
                    if (p.IsTextEnabled && p.TextEmail.Length > 0)
                        Message.Recipients.Add(p.TextEmail, string.Empty, RecipientType.BCC);
                }
            }

            // Add body parts
            Message.BodyParts.Add(getTextMessageBody(includeFieldStatus, extraMessage), BodyPartFormat.Plain);

            SMTP server = new SMTP("");
            server.Send(Message);
        }

        private void SendSmsEmail(bool includeFieldStatus, string extraMessage, IEnumerable<Player> players)
        {
            string teamName = _repository.GetTeamName();

            EmailMessage Message = new EmailMessage();
            Message.From.Email = "";
            Message.From.Name = teamName + " Update";
            Message.Subject = teamName + " Update";

            foreach (Player p in players)
            {
                if (p.PlayerID == 1)
                    Message.Recipients.Add(p.Email, p.FirstName + " " + p.LastName, RecipientType.To);

                //if (p.IsEmailEnabled && p.Email.Length > 0)
                //    Message.Recipients.Add(p.Email, p.FirstName + " " + p.LastName, RecipientType.BCC);

                if (p.IsTextEnabled && p.TextEmail.Length > 0)
                    Message.Recipients.Add(p.TextEmail, string.Empty, RecipientType.BCC);
            }

            // Add body parts
            Message.BodyParts.Add(getTextMessageBody(includeFieldStatus, extraMessage), BodyPartFormat.Plain);

            SMTP server = new SMTP("");
            server.Send(Message);
        }

        private string getTextMessageBody(bool includeGameStatus, string extraMessage)
        {
            string teamName = _repository.GetTeamName();
            string result = string.Empty;

            if (includeGameStatus)
            {
                //DateTime targetDate = DateTime.Now.Date;
                var game = _repository.FindNextGames(targetDate).Single();
                //var obj = new Game();

                //obj.Query.TopN = 1;
                //obj.Where.GameDate.Value = DateTime.Now;
                //obj.Where.GameDate.Operator = NCI.EasyObjects.WhereParameter.Operand.GreaterThan;
                //obj.Query.Load();

                if (game.AtHomeFirst)
                    result = string.Format("{0} vs. {1}*", game.Opponent, teamName);
                else
                    result = string.Format("{0}* vs. {1}", game.Opponent, teamName);

                result += string.Format("\n{0:MMMM d @ h:mm tt}\n{1}\nField status is '{2}'",
                            game.GameDate, game.GameLocation,
                            (string.IsNullOrEmpty(game.GameStatus) ? "UNKNOWN" : game.GameStatus));
            }

            if (!string.IsNullOrWhiteSpace(extraMessage))
                result += "\n\n" + extraMessage;

            return result.Trim().Left(140);
        }

        private string GetFieldStatus(string p, string token)
        {
            int pos = p.IndexOf(token);
            string result = string.Empty;

            if (pos == -1) return "UNKNOWN";

            for (int i = pos; i < p.Length; i++)
            {
                string c = p.Substring(i, 1);
                if (c == "\r" || c == "\n")
                    break;
                result += c;
            }

            return result.Trim();
        }

        private FieldStatus GetFieldStatusFromDom(string html, string token)
        {
            var doc = new HtmlDocument();
            doc.Load(new StringReader(html));

            var node = doc.DocumentNode.SelectSingleNode("//span[@class='cwpdnote']");
            DateTime updateDate;
            string status = "UNKNOWN";

            if (!DateTime.TryParse(node.InnerText.Replace("Updated ", "").Trim(), out updateDate))
                updateDate = DateTime.Now;

            var nodes = doc.DocumentNode.SelectNodes("//table[@width='445' and @cellpadding='3']");
            foreach (var n in nodes)
            {
                string p = n.InnerText.Trim();
                int pos = p.IndexOf(token);
                if (pos >= 0)
                {
                    string result = string.Empty;
                    for (int i = pos; i < p.Length; i++)
                    {
                        string c = p.Substring(i, 1);
                        if (c == "\r" || c == "\n")
                            break;
                        result += c;
                    }

                    if (!string.IsNullOrWhiteSpace(result))
                        status = result;

                    break;
                }
            }

            return new FieldStatus
            {
                UpdateDate = updateDate,
                Status = status
            };
        }

        private void DoUpdateFieldStatus()
        {
            //DateTime targetDate = DateTime.Now.Date;
            //DateTime endDate = DateTime.Now.AddDays(7.0);

            var game = _repository.FindNextGames(targetDate).SingleOrDefault();

            if (game != null)
            {
                string token = game.GameLocation.Left(1) + game.GameLocation.Right(1) + " - ";
                string response = Util.GetUrl("http://www.cwpd.org/field_status.shtml");
                //string fieldStatus = this.GetFieldStatus(Util.stripHtml(response), token);
                var fieldStatus = GetFieldStatusFromDom(response, token);

                game.GameStatus = fieldStatus.Status;
                _repository.SaveChanges();
            }
        }

    }
}
