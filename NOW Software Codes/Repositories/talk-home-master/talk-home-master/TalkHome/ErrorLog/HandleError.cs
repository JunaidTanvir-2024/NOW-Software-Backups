using System.Web.Mvc;

namespace TalkHome.ErrorLog
{
    public class HandleErrorTHM : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            ErrorLog.LogError(filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}