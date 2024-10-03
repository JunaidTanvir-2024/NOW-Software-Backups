using System.Collections.Generic;
using TalkHome.Models.WebApi.DTOs;
using TalkHome.Models.WebApi.History;

namespace TalkHome.Models.ViewModels
{
    /// <summary>
    /// Defines the view model for My Account page
    /// </summary>
    public class MyAccountViewModel
    {
        public JWTPayload Payload { get; set; }

        public string ProductCode { get; set; }

        public AccountSummaryResponseDTO AccountSummary { get; set; }

        public List<CallHistoryRecord> CallRecords { get; set; } = new List<CallHistoryRecord>();

        public MyAccountViewModel(JWTPayload payload, string productCode, AccountSummaryResponseDTO accountSummary, List<CallHistoryRecord> callRecords)
        {
            Payload = payload;

            ProductCode = productCode;

            AccountSummary = accountSummary;

            CallRecords = callRecords;
        }

    }
}
