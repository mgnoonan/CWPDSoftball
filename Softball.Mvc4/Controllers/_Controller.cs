using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.InteropServices;
using System.Web.Caching;

namespace Softball.Mvc4.Controllers
{
    public class _Controller : Controller
    {
        public virtual DateTime targetDate
        {
            get { return DateTime.Now; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Set the machine name so we can display in Site.master
            ViewBag.MachineName = Environment.MachineName;

            // Set the assembly version so we can display in Site.master
            ViewBag.AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            // Set info about the request
            ViewBag.UserAgent = filterContext.HttpContext.Request.UserAgent;
            ViewBag.IsMobileDevice = filterContext.HttpContext.Request.Browser.IsMobileDevice;
        }
    }
}