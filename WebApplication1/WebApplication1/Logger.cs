using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace WebApplication1
{
    public class Logger
    {
        public Logger()
        {
        }

        private static readonly object syncObject = new object();

        public static bool AddToLogFile(string logContent, string logFileName)
        {
            return AddToLogFile(logContent, logFileName, false);
        }

        public static bool AddToLogFile(string logContent, string logFileName, bool addLineBreak)
        {
            DateTime now = DateTime.Now;

            if (addLineBreak)
                logContent += Environment.NewLine;

            string path = null;
            try
            {
                path = HttpContext.Current.Server.MapPath("~/App_Data/ErrorLogs/" + now.ToString("yyyy-MM-dd") + "/");
            }
            catch
            {
                path = null;
            }

            if (string.IsNullOrEmpty(path) == false)
            {
                logFileName = path + logFileName;
                lock (syncObject)
                {
                    try
                    {
                        if (Directory.Exists(path) == false)
                            Directory.CreateDirectory(path);
                        File.AppendAllText(logFileName, logContent, Encoding.UTF8);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }
    }
}