using System;
using System.Web;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;

namespace Softball.Mvc4.Utilities
{
	/// <summary>
	/// Summary description for HttpPost.
	/// </summary>
	public class HttpPost
	{
		public static ManualResetEvent _allDone = new ManualResetEvent(false);
		private static string _postData = string.Empty;
		private static bool _zipRequest = true;
		private static bool _zipResponse = true;
		private static string _userAgent = string.Empty;

		public HttpPost() {}

		public static string Post2(string url)
		{
			// first, request the login form to get the viewstate value
			HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;         
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.CookieContainer = new CookieContainer();        
			webRequest.UserAgent = _userAgent;
			webRequest.Timeout = 30000;

			// write the form values into the request message
//			StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
//			requestWriter.Write(postData);
//			requestWriter.Close();

			WebResponse response = webRequest.GetResponse();
			StreamReader responseReader = new StreamReader(response.GetResponseStream());
   
			// and read the response
			string responseData = responseReader.ReadToEnd();
			responseReader.Close();
   
			return responseData;
		}

		public static string Post(string url)
		{
			return Post(url, string.Empty, Timeout.Infinite);
		}

		public static string Post(string url, int timeout)
		{
			return Post(url, string.Empty, timeout);
		}

		public static string Post(string url, string postData)
		{
			return Post(url, postData, Timeout.Infinite);
		}

		public static string Post(string url, string postData, int timeout)
		{
			Uri httpSite = new Uri(url);
			_postData = postData;

			// Create a new request to the URL.    
			HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(httpSite);
			myWebRequest.CookieContainer = new CookieContainer();
            myWebRequest.UserAgent = _userAgent;

			// Create an instance of the RequestState and assign 'myWebRequest' to it's request field.    
			RequestState myRequestState = new RequestState();

			//ServicePointManager.CertificatePolicy = new MyPolicy();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			myRequestState.Request = myWebRequest;            
			myWebRequest.ContentType = "text/plain";

			// Set the 'Method' prperty  to 'POST' to post data to a Uri.
			myRequestState.Request.Method = "POST";
			myRequestState.Request.ContentType = "application/x-www-form-urlencoded";
			myRequestState.Request.Timeout = timeout;
			myRequestState.Request.Headers.Add("Accept-Encoding", "gzip, deflate");

			// Start the Asynchronous 'BeginGetRequestStream' method call.    
			IAsyncResult r = (IAsyncResult) myWebRequest.BeginGetRequestStream(new AsyncCallback(ReadCallback), myRequestState);            

			// Assign the response object of 'WebRequest' to a 'WebResponse' variable.
			WebResponse myWebResponse = myWebRequest.GetResponse();

			Stream streamResponse = myWebResponse.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

			StringBuilder responseData = new StringBuilder();
			int bytes = 0;
			int bufferSize = 256;
			Char[] readBuff = new Char[bufferSize];
			int count = streamRead.Read( readBuff, 0, bufferSize );

			while (count > 0) 
			{
				bytes += count;
				responseData.Append( new String(readBuff, 0, count) );
				count = streamRead.Read(readBuff, 0, bufferSize);
			}

			// Close the Stream Object.
			streamResponse.Close();
			streamRead.Close();
			_allDone.WaitOne();    

			// Release the HttpWebResponse Resource.
			myWebResponse.Close();

			return responseData.ToString();
		}

		public static string MySource(Uri weburi) 
		{
			System.Text.StringBuilder sbuild=new StringBuilder();
			string temp=""; 
			try 
			{ 
				System.Net.HttpWebRequest webrequest = (HttpWebRequest)System.Net.WebRequest.Create(weburi); 
				System.Net.HttpWebResponse webresponse=(HttpWebResponse) webrequest.GetResponse(); 
				StreamReader webstream = new StreamReader(webresponse.GetResponseStream(),Encoding.ASCII );
				while((temp=webstream.ReadLine())!= null) 
				{
					sbuild.Append(temp + "\r\n");
				}
				webstream.Close(); 

				return sbuild.ToString();
			}
			catch { return string.Empty; }
		}

		public static bool ZipRequest
		{
			get { return _zipRequest; }
			set { _zipRequest = value; }
		}

		public static bool ZipResponse
		{
			get { return _zipResponse; }
			set { _zipResponse = value; }
		}

		public static string UserAgent
		{
			get { return _userAgent; }
			set { _userAgent = value; }
		}

		private static void ReadCallback(IAsyncResult asynchronousResult)
		{    
			try
			{
				// State of request is set to asynchronous.
				RequestState myRequestState = (RequestState) asynchronousResult.AsyncState;
				WebRequest  myWebRequest2 = myRequestState.Request;

				// End of the Asynchronus request.
				Stream streamResponse = myWebRequest2.EndGetRequestStream(asynchronousResult);

				// Create a string that is to be posted to the uri.
				string postData = _postData;
				ASCIIEncoding encoder = new ASCIIEncoding();

				// Convert  the string into a byte array.
				byte[] ByteArray = encoder.GetBytes(postData);

				// Write data to the stream.
				streamResponse.Write(ByteArray, 0, postData.Length);
				streamResponse.Close();        
				_allDone.Set();
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
	}


	public class MyPolicy : ICertificatePolicy
	{
		#region ICertificatePolicy Members

		public bool CheckValidationResult(ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, WebRequest request, int certificateProblem)
		{
			// NOTE: Currently, we ignore any certificate errors by just returning true
			return true;
		}

		#endregion
	}


	// The RequestState class passes data across async calls.
	public class RequestState
	{
		const int BufferSize = 1024;
		public StringBuilder RequestData;
		public byte[] BufferRead;
		public WebRequest Request;
		public Stream ResponseStream;
		// Create Decoder for appropriate enconding type.
		public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
      
		public RequestState()
		{
			BufferRead = new byte[BufferSize];
			RequestData = new StringBuilder(String.Empty);
			Request = null;
			ResponseStream = null;
		}     
	}
}
