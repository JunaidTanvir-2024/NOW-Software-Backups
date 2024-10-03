using System;

namespace TalkHome.Logger
{
    public interface ILoggerService
    {
        void SendCriticalAlert(int code);

        void SendInfoAlert(string message);

        void SendTransactionRecord(int code);

        void Error(Type type, string message, Exception e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");

        void Warn(Type type, string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");

        void Warn(Type type, object obj, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");

        void Debug(Type type, string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");

        void Debug(Type type, object obj, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");

        void Info(Type type, string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");

        void Info(Type type, object obj, [System.Runtime.CompilerServices.CallerMemberName] string caller = "");
    }
}
