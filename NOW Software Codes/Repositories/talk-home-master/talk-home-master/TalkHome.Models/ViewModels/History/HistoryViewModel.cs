using System.Collections.Generic;
using TalkHome.Models.WebApi.Rates;

namespace TalkHome.Models.ViewModels.History
{
    /// <summary>
    /// The view model for history records
    /// </summary>
    /// <typeparam name="T">The Type of records</typeparam>
    public class HistoryViewModel<T>
    {
        public JWTPayload Payload { get; set; }

        public int Parameter { get; set; }

        public string ProductCode { get; set; }

        public List<T> Records { get; set; } = new List<T>();

        public TotalPages TotalPages { get; set; }

        public HistoryViewModel(JWTPayload payload, int parameter, string productCode, List<T> records, TotalPages totalPages)
        {
            Payload = payload;

            Parameter = parameter;

            ProductCode = productCode;

            Records = records;

            TotalPages = totalPages;
        }

        public HistoryViewModel(JWTPayload payload)
        {
            Payload = payload;
        }
    }
}
