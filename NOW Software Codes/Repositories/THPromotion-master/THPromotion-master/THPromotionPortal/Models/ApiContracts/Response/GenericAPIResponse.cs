using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THPromotionPortal.Models.enums;

namespace THPromotionPortal.Models.ApiContracts.Response
{
    public class GenericApiResponse<T>
    {

        public string message { get; set; }
        public string status { get; set; }
        public int errorCode { get; set; }
        public T payload { get; set; }
        public static GenericApiResponse<T> Success(T payload, string message)
        {
            return new GenericApiResponse<T>
            {
                errorCode = 0,
                status = "Success",
                message = message,
                payload = payload
            };
        }
        public static GenericApiResponse<T> Success(string message)
        {
            return new GenericApiResponse<T>
            {
                errorCode = 0,
                status = "Success",
                message = message
            };
        }
        public static GenericApiResponse<T> Failure(string message, ApiStatusCodes errorCode)
        {
            return new GenericApiResponse<T>
            {
                errorCode = (int)errorCode,
                status = "Failure",
                message = message

            };
        }
        public static GenericApiResponse<T> Failure(T payload, string message, ApiStatusCodes errorCode)
        {
            return new GenericApiResponse<T>
            {
                errorCode = (int)errorCode,
                status = "Failure",
                message = message,
                payload = payload
            };
        }
    }
}
