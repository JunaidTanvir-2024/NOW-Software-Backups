using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TalkHome.Logger;
using TalkHome.Models;

namespace TalkHome.Extensions.Html
{
    public static class MailExtentions
    {
        public static async System.Threading.Tasks.Task CreditSimSuccessMailAsync(JWTPayload Payload)
        {
            Dictionary<string, string> substitutions = new Dictionary<string, string>();
    
            if (!string.IsNullOrEmpty(Payload.EmailVerificationToken))
            {
                string verifyLink = System.Configuration.ConfigurationManager.AppSettings["SignUpConfirmationPageUrl"];

                substitutions.Add("%VERIFY_LINK%", String.Format(verifyLink, Payload.EmailVerificationToken));
            }

            MailTemplate mailTemplate = new MailTemplate
            {
                Template = string.IsNullOrEmpty(Payload.EmailVerificationToken) ? 
                        MailTemplate.CREDIT_SIM_SUCCESS_TEMPLATE : MailTemplate.CREDIT_SIM_SUCCESS_TEMPLATE_SIGNUP,
                EmailAddress = Payload.CreditSim.Email,
                Substitutions = substitutions,
                From = System.Configuration.ConfigurationManager.AppSettings["EmailFrom_ForgotPassword"],
                Host = System.Configuration.ConfigurationManager.AppSettings["EmailHost"],
                Subject = "Your Talk Home Mobile SIM order confirmation"
            };

            try
            {
                await mailTemplate.Send();
            }
            catch (Exception e)
            {
            
            }

        }

    }
}