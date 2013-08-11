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
        [DllImport("netapi32.dll", CharSet = CharSet.Auto)]
        static extern int NetWkstaGetInfo(string server,
            int level,
            out IntPtr pBuf);

        [DllImport("netapi32.dll")]
        static extern int NetApiBufferFree(IntPtr pBuf);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class WKSTA_INFO_100
        {
            public int wki100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_computername;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_langroup;
            public int wki100_ver_major;
            public int wki100_ver_minor;
        }

        private string _machineName = null;
        public virtual string MachineName
        {
            get
            {
                WKSTA_INFO_100 info;
                IntPtr pBuffer = IntPtr.Zero;
                string _machineName;

                try
                {
                    int retval = NetWkstaGetInfo(null, 100, out pBuffer);
                    if (retval == 0)
                    {
                        info = (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
                        _machineName = info.wki100_computername;
                    }
                    else
                    {
                        _machineName = "00";
                    }

                    return _machineName;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (pBuffer != IntPtr.Zero)
                        NetApiBufferFree(pBuffer);
                }
            }
        }

        public virtual DateTime targetDate
        {
            get { return DateTime.Now; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Set the machine name so we can display in Site.master
            ViewBag.MachineName = this.MachineName;

            // Set the assembly version so we can display in Site.master
            ViewBag.AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            // Set info about the request
            ViewBag.UserAgent = filterContext.HttpContext.Request.UserAgent;
            ViewBag.IsMobileDevice = filterContext.HttpContext.Request.Browser.IsMobileDevice;
        }
    }
}