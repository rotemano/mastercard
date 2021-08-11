using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;

namespace WebApplication1
{


    /// <summary>
    /// Summary description for ExceptionsLogger
    /// </summary>
    public class ExceptionsLogger
    {
        public ExceptionsLogger()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static void LogException(Exception ex)
        {
            LogException(ex, null);
        }

        public static void LogException(Exception ex, Dictionary<string, string> customParams)
        {
            bool logExceptionsToFile = true;

            CustomException ce = new CustomException(ex);
            if (customParams != null)
                ce.customParams = customParams;

            if (logExceptionsToFile)
                LogExceptionToFile(ce);
        }


        private static void LogExceptionToFile(CustomException ce)
        {
            string exceptionLogFile = "ExceptionLog.html";
            string exceptionEntry = GetExceptionEntry(ce);
            Logger.AddToLogFile(exceptionEntry, exceptionLogFile);
        }

        private static string GetExceptionEntry(CustomException ce)
        {
            DateTime now = DateTime.Now;

            StringBuilder msg = new StringBuilder();
            msg.Append("<table style='font-family: arial;font-size:15px;'>");
            AddLogLine(msg, "Exception Date", now.ToString("yyyy-MM-dd HH\\:mm\\:ss"));
            AddLogLine(msg, "Page Name", ce.pageName);
            AddLogLine(msg, "Request URL", ce.requestURL);
            AddLogLine(msg, "Referer", ce.httpReferer);
            AddLogLine(msg, "Exception Type", ce.exceptionType);
            AddLogLine(msg, "Exception Message", HttpUtility.HtmlEncode(ce.exceptionMessage != null ? ce.exceptionMessage : string.Empty).Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;"));
            AddLogLine(msg, "Exception Stack Frame", HttpUtility.HtmlEncode(ce.exceptionStackFrame).Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;"));
            AddLogLine(msg, "Exception Stack Trace", HttpUtility.HtmlEncode((ce.exceptionStackTrace != null ? ce.exceptionStackTrace : string.Empty).Replace("   ", string.Empty)).Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;"));
            AddLogLine(msg, "Exception Source", ce.exceptionSource);
            AddLogLine(msg, "Exception Method", ce.exceptionMethod);

            foreach (InnerException ix in ce.innerExceptions)
            {
                msg.Append("<tr><td>Inner Exception " + ix.exceptionIndex + "</td><td><table style='font-family: arial;font-size:15px;'>");
                AddLogLine(msg, "Exception Type", ix.exceptionType);
                AddLogLine(msg, "Exception Message", HttpUtility.HtmlEncode(ix.exceptionMessage != null ? ix.exceptionMessage : string.Empty).Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;"));
                AddLogLine(msg, "Exception Stack Frame", HttpUtility.HtmlEncode(ix.exceptionStackFrame).Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;"));
                AddLogLine(msg, "Exception Stack Trace", HttpUtility.HtmlEncode((ix.exceptionStackTrace != null ? ix.exceptionStackTrace : string.Empty).Replace("   ", string.Empty)).Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;"));
                AddLogLine(msg, "Exception Method", ix.exceptionMethod);
                msg.Append("</table></td></tr>");
            }

            msg.Append("<tr><td colspan='2'><b>Postback Control:</b>&nbsp;" + ce.postBackControlDescription + "</td></tr>");

            if (ce.customParams != null)
            {
                foreach (KeyValuePair<string, string> kv in ce.customParams)
                {
                    AddLogLine(msg, kv.Key, kv.Value);
                }
            }

            msg.Append("</table><br/><hr/><br/>");
            msg.Append(Environment.NewLine);

            return msg.ToString();
        }

        #region Helper Methods

        private static void AddLogLine(StringBuilder msg, string header, string content)
        {
            msg.Append("<tr><td valign='top' nowrap='nowrap'><b>");
            msg.Append(header);
            msg.Append(":&nbsp;&nbsp;&nbsp;</b></td><td>");
            msg.Append(content);
            msg.Append("</td></tr>");
        }

        private static string GetExceptionStackFrame(Exception ex)
        {
            StackTrace st = new StackTrace(ex, true);
            StackFrame sf = null;
            StringBuilder sb = new StringBuilder();
            int lineNumber = 0;
            for (int i = 0; i < st.FrameCount; i++)
            {
                sf = st.GetFrame(i);
                sb.Append(sf.GetMethod().ReflectedType.FullName);
                sb.Append(".");
                sb.Append(sf.GetMethod().Name);
                sb.Append((sf.GetMethod().MemberType == MemberTypes.Method ? "()" : string.Empty));
                lineNumber = sf.GetFileLineNumber();
                if (lineNumber > 0)
                {
                    sb.Append(" at line ");
                    sb.Append(lineNumber);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private static string GetExceptionMethod(Exception ex)
        {
            MethodBase methodBase = ex.TargetSite;
            if (methodBase == null)
                return null;

            return
                methodBase.ReflectedType.Name + "." +
                methodBase.Name +
                (methodBase.MemberType == MemberTypes.Method ? "()" : string.Empty);
        }

        #endregion

        #region Helper Classes

        private class CustomException
        {
            public string exceptionType { get; set; }
            public string exceptionMessage { get; set; }
            public string exceptionStackFrame { get; set; }
            public string exceptionStackTrace { get; set; }
            public string exceptionSource { get; set; }
            public string exceptionMethod { get; set; }
            public List<InnerException> innerExceptions { get; set; }
            public string requestURL { get; set; }
            public string pageName { get; set; }
            public string httpReferer { get; set; }
            public string postBackControlDescription { get; set; }
            public Dictionary<string, string> customParams { get; set; }

            public CustomException(Exception ex)
            {
                this.exceptionType = ex.GetType().Name;
                this.exceptionMessage = ex.Message;
                this.exceptionStackFrame = GetExceptionStackFrame(ex);
                this.exceptionStackTrace = ex.StackTrace;
                this.exceptionSource = ex.Source;
                this.exceptionMethod = GetExceptionMethod(ex);

                ex = ex.InnerException;
                int innerExceptionCounter = 0;
                innerExceptions = new List<InnerException>();
                while (ex != null)
                {
                    innerExceptions.Add(new InnerException(++innerExceptionCounter, ex));
                    ex = ex.InnerException;
                }


            }
        }

        private class InnerException
        {
            public int exceptionIndex { get; set; }
            public string exceptionType { get; set; }
            public string exceptionMessage { get; set; }
            public string exceptionStackTrace { get; set; }
            public string exceptionStackFrame { get; set; }
            public string exceptionMethod { get; set; }

            public InnerException(int exceptionIndex, Exception ex)
            {
                this.exceptionIndex = exceptionIndex;
                this.exceptionType = ex.GetType().Name;
                this.exceptionMessage = ex.Message;
                this.exceptionStackTrace = ex.StackTrace;
                this.exceptionStackFrame = GetExceptionStackFrame(ex);
                this.exceptionMethod = GetExceptionMethod(ex);
            }
        }

        #endregion
    }
}