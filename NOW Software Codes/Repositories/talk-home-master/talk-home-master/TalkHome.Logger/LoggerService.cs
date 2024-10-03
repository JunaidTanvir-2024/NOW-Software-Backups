using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Web;
using Umbraco.Core.Logging;

namespace TalkHome.Logger
{
    /// <summary>
    /// Contains logging methods for the application.
    /// Note that zabbix sender exe file is included with the distrubution of the application. For local logs, we realy on Umbraco's LogHelper set up
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private Properties.Zabbix Settings = Properties.Zabbix.Default;

        /// <summary>
        /// Sends the alert via the Zabbix sender .exe file
        /// </summary>
        /// <param name="arguments">The argument string</param>
        private void SendAlert(string arguments)
        {
            string ExePath = string.Format("{0}{1}", HttpRuntime.AppDomainAppPath, Settings.DebugExePath);

            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.CreateNoWindow = false;
            StartInfo.UseShellExecute = false;
            StartInfo.FileName = ExePath;
            StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            StartInfo.Arguments = arguments;

            try
            {
                using (Process ExeProcess = Process.Start(StartInfo))
                {
                    ExeProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Error(GetType(), "Zabbix sender failed to send an alert.", e);
            }
        }

        /// <summary>
        /// Formats argument string for Zabbix critical alerts. These are actively monitored!
        /// </summary>
        /// <param name="code">The error code</param>
        public void SendCriticalAlert(int code)
        {
            var Arguments = string.Format("-z {0} -s {1} -k {2} -o {3}", Settings.Ip, Settings.Host, Settings.CriticalSeverity, code);

            SendAlert(Arguments);
        }

        /// <summary>
        /// Formats arguments for Zabbix info alerts. Not actively monitored
        /// </summary>
        /// <param name="message">The message</param>
        public void SendInfoAlert(string message)
        {
            var Arguments = string.Format("-z {0} -s {1} -k {2} -o \"{3}\"", Settings.Ip, Settings.Host, Settings.InfoSeverity, message);

            SendAlert(Arguments);
        }

        /// <summary>
        /// Formats arguments for Zabbix transaction records. Not actively monitored
        /// </summary>
        /// <param name="code">The outcome code</param>
        public void SendTransactionRecord(int code)
        {
            var Arguments = string.Format("-z {0} -s {1} -k {2} -o {3}", Settings.Ip, Settings.Host, Settings.CriticalSeverity, code);

            SendAlert(Arguments);
        }

        /// <summary>
        /// Creates an ERROR log with exception
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="e">The thrown exception</param>
        /// <param name="caller">The caller method</param>
        public void Error(Type type, string message, Exception e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Error(type, message, e);
        }

        /// <summary>
        /// Creates a WARN log
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="caller">The caller method</param>
        public void Warn(Type type, string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Warn(type, string.Format("{0} - {1}", caller, message.Replace("{", "{{").Replace("}", "}}")));
        }

        /// <summary>
        /// Creates a WARN log
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="caller">The caller method</param>
        public void Warn(Type type, object obj, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Warn(type, string.Format("{0} - {1}", caller, JsonConvert.SerializeObject(obj).Replace("{", "{{").Replace("}", "}}")));
        }

        /// <summary>
        /// Creates a DEBUG log
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="caller">The caller method</param>
        public void Debug(Type type, string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Debug(type, string.Format("{0} - {1}", caller, message.Replace("{", "{{").Replace("}", "}}")));
        }

        /// <summary>
        /// Creates a DEBUG log
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="caller">The caller method</param>
        public void Debug(Type type, object obj, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Debug(type, string.Format("{0} - {1}", caller, JsonConvert.SerializeObject(obj).Replace("{", "{{").Replace("}", "}}")));
        }

        /// <summary>
        /// Creates an INFO log
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="caller">The caller method</param>
        public void Info(Type type, string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Info(type, string.Format("{0} - {1}", caller, message.Replace("{", "{{").Replace("}", "}}")));
        }

        /// <summary>
        /// Creates an INFO log
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="message">The message</param>
        /// <param name="caller">The caller method</param>
        public void Info(Type type, object obj, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            LogHelper.Info(type, string.Format("{0} - {1}", caller, JsonConvert.SerializeObject(obj).Replace("{", "{{").Replace("}", "}}")));
        }
    }
}
