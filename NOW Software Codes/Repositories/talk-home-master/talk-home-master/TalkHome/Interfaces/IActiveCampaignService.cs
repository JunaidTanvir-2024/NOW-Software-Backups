using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkHome.Models;
using TalkHome.Services;

namespace TalkHome.Interfaces
{
    public interface IActiveCampaignService
    {
        Task<ActiveCampaignResult> AddTag(string tag, string emailAddress);
        Task<ActiveCampaignResult> GetContactDetails(string emailAddress);
        Task<ActiveCampaignResult> AddToList(string listId, string emailAddress, string firstName, string lastName);
        Task<ActiveCampaignResult> RemoveTag(string tag, string emailAddress);
        Task<ActiveCampaignResult> ChangeSubscriptionStatus(string listId, string emailAddress, string id, string status);
        Task<ActiveCampaignResult> DeleteContact(string emailAddress);
    }

}
