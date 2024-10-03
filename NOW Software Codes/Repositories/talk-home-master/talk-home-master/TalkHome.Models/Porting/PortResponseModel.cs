using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHome.Models.Porting
{
    public class PortResponseModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string NTMsisdn { get; set; }
        public string PortMsisdn { get; set; }
        public string Code { get; set; }
        public CodeTypes CodeType { get; set; }
        public string Status { get; set; }
        public string StatusNote { get; set; }
        public DateTime? UserPortingDate { get; set; }
        public DateTime? PortDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public PortTypes PortType { get; set; }
        public Products Product { get; set; }
        public MediumTypes RequestMediumType { get; set; }
        public bool IsProcessed { get; set; }
        public SwitchingInfoResponseModel SwitchingInfo { get; set; }
        public int UserId { get; set; }
        public DateTime? ProcessedDateTime { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public bool IsError { get; set; }
        public string DaemonErrorMessage { get; set; }
        public string DaemonPortInRequestJson { get; set; }
        public string DaemonPortInResponseJson { get; set; }
        public string DaemonPortOutRequestJson { get; set; }
        public string DaemonPortOutResponseJson { get; set; }
    }
}
