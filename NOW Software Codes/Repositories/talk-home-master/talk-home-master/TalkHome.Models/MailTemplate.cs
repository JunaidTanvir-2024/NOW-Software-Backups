using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web;
using System.Configuration;



namespace TalkHome.Models
{
   
    public class MailTemplate
    {
        public const string VERIFY_EMAIL_TEMPLATE = "Verify.html";
        public const string TRANSFER_EMAIL_TEMPLATE = "TransferEmailTemplate.html";
        public const string EXCEPTION_TEMPLATE = "ExceptionTemplate.html";
        public const string MIGRATION_TEMPLATE = "MigrationTemplate.html";
        public const string FORGOT_PASSWORD = "ForgotPasswod.html";
        public const string ORDER_SIM = "Order_Sim.html";
        public const string CREDIT_SIM_SUCCESS_TEMPLATE = "CreditSimSuccess.html";
        public const string ORDER_SIM_WITH_SIGNUP = "Order_Sim_SignUp.html";
        public const string CREDIT_SIM_SUCCESS_TEMPLATE_SIGNUP = "CreditSimSuccess_SignUp.html";
        public const string otp_TEMPLATE = "Otp.html";

        public string Host { get; set; }
        public string From { get; set; }
        public string Template { get; set; }
        public Dictionary<string,string> Substitutions { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }

        
        public MailTemplate()
        {
         
        }

        public async Task Send()
        {
         
            string messageBody = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/App_Data/EmailTemplates/" + Template));

            foreach (var s in Substitutions) {
                messageBody = messageBody.Replace(s.Key, s.Value);
            }

            MailMessage message = new MailMessage(From, EmailAddress, Subject, messageBody);
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(Host);
            await client.SendMailAsync(message);
        }


        public void SyncSend()
        {

            string messageBody = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/App_Data/EmailTemplates/" + Template));

            foreach (var s in Substitutions)
            {
                messageBody = messageBody.Replace(s.Key, s.Value);
            }

            MailMessage message = new MailMessage(From, EmailAddress, Subject, messageBody);
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(Host);
            client.Send(message);
        }
    }
}
