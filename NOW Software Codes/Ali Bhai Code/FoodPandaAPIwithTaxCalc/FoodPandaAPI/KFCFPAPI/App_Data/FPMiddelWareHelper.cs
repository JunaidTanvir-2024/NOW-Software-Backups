using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using KFCFPAPI.Models;
using KFCDB;
using Newtonsoft.Json;
using KFCFPAPI.App_Start;
using System.Net;
using System.Net.Security;
using Newtonsoft.Json.Linq;

namespace KFCFPAPI.AppData
{

    public class FPMiddelWareHelper
    {
        public string BranchId { get; set; }

        public Branch hdsBranch { get; set; }

        public int FoodPandaDelivery { get; set; }

        public int ProductPrice { get; set; }

        public string AccessToken { get; set; }

        public FoodPandaPayload FoodPandaPaylaod { get; set; }

        public string Instructions { get; set; }
        List<string> paymethod = new List<string>() { "pending", "paid" };

        private string SaveFPOrderPayLoad(string branchId, object orderRequest, string apiType, string orderNo)
        {
            string serverIp = ConfigurationManager.AppSettings["Target"].ToString();
            if (orderRequest != null)
            {
                string payload = JsonConvert.SerializeObject(orderRequest);
                FoodPandaPayload val = new FoodPandaPayload()
                {
                    cd = DateTime.Now,
                    Payload = payload,
                    branchId = branchId,
                    ServerIP = serverIp,
                    apiType = apiType,
                    orderNo = orderNo
                };

                using (KFC_PortalEntities entity = new KFC_PortalEntities())
                {
                    entity.FoodPandaPayloads.Add(val);
                    entity.SaveChanges();
                    FoodPandaPaylaod = val;
                }
                
                return "Success";
            }
            return string.Empty;
        }

        public string ProcessDataAsync(string branchId, FPOrder order)
        {
            DateTime startTime = DateTime.Now;
            Instructions = string.Empty;
            try
            {
                SaveFPOrderPayLoad(branchId, order, "Create Order", "-1");
                BranchId = branchId;
                if (order == null)
                {
                    return JsonConvert.SerializeObject(new FailureResponse
                    {
                        reason = "Invalid Json",
                        message = "Unable to Validate Order"
                    });
                }
                string valiationResponse = ValidateFPOrder(order);
                if (valiationResponse == "Success")
                {
                    ConfigurationManager.AppSettings["Target"].ToString();
                    string deliveryAddress = getDeliveryAddress(order.delivery.address);
                    string paymentMethod = setPayMethod(order.payment);
                    string Instructions = string.Empty;
                    if (order.comments != null && !string.IsNullOrEmpty(order.comments.customerComment) && order.comments.customerComment.Trim().ToLower() != "null" && !order.comments.customerComment.Trim().Equals("- ** DO NOT PROVIDE CUTLERY"))
                    {
                        Instructions = order.comments.customerComment;
                    }
                    if (order.comments != null && order.comments.vendorComment != null && !string.IsNullOrEmpty(order.comments.vendorComment) && order.comments.vendorComment.ToLower().Trim() != "null")
                    {
                        Instructions = Instructions + " " + order.comments.vendorComment;
                    }
                    //if (!string.IsNullOrEmpty(this.Instructions))
                    //{
                    //    Instructions = Instructions + " " + this.Instructions;
                    //}
                    string isPickUp = order.expeditionType.ToLower().Trim().Equals("pickup") ? "1" : "0";
                    order.expeditionType.ToLower().Trim().Equals("pickup");
                    List<FPOrderItemsDetails> itemsList = getItemsList(order.products);
                    string itemsLists = JsonConvert.SerializeObject(itemsList);
                    bool isTakeAway = isPickUp == "1" ? true : false;
                    //if (!order.expeditionType.Trim().ToLower().Equals("pickup"))
                    //{
                    //    string.IsNullOrEmpty(order.delivery.riderPickupTime);
                    //}
                    using (KFC_PortalEntities entity = new KFC_PortalEntities())
                    {
                        entity.Database.CommandTimeout = 300;
                        ObjectResult objectResult = entity.Proc_IsOrderExistByCIN_New(order.code);
                        long num = 0;
                        foreach (long item in objectResult)
                        {
                            num = item;
                        }
                        if (num != 0)
                        {
                            RejectOrderDueToDupliction(order.token);
                            return JsonConvert.SerializeObject(new FailureResponse
                            {
                                reason = "MENU_ACCOUNT_SETTINGS",
                                message = "Duplicate Order"
                            });
                        }
                        int deliveryCharges = getDeliveryCharges(order?.price?.deliveryFee ?? string.Empty);
                        long customerIdTemp = AddCustomerTemp(order.customer, deliveryAddress);
                        string regionId = hdsBranch?.regionID.ToString() ?? "0";
                        string bId = hdsBranch?.BranchID.ToString() ?? "0";
                        int timeExtenstion = hdsBranch?.TimeExtension ?? 30;
                        deliveryAddress = string.IsNullOrEmpty(deliveryAddress) ? branchId  : deliveryAddress;
                        

                        //ObjectResult orderNumberTemp = entity.Proc_SaveFoodPandaOrderTemp_New(string.Empty, string.Empty, order.code, bId, paymentMethod, 
                        //    (long?)customerIdTemp, order.price.totalNet, deliveryAddress, Instructions, (bool?)isTakeAway, deliveryCharges.ToString(), 
                        //    hdsBranch?.TimeExtension?.ToString() ?? "30", "Food Panda", itemsLists, regionId, 
                        //    order.delivery.address?.city ?? string.Empty, "Food Panda", string.Empty, string.Empty, isPickUp, 
                        //    order.delivery?.address?.latitude ?? string.Empty, order.delivery?.address?.longitude ?? string.Empty, order.expeditionType, order?.token);
                        ObjectResult orderNumberTemp = entity.Proc_SaveFoodPandaOrderTemp_New_One(onlineDeleiveryOrderID: string.Empty, orderNo: string.Empty, cIN: order.code, branch: bId,
                            paymentMode: paymentMethod, customerID: customerIdTemp, amount: order.price.totalNet, deliveryAddress: deliveryAddress, instruction: Instructions,
                            takeAway: isTakeAway, serviceCharges: deliveryCharges.ToString(), timeRequired: timeExtenstion.ToString(),
                            orderThrough: "Food Panda", itemDetailArray: itemsLists, regionID: regionId, city: order?.delivery?.address?.city ?? string.Empty,
                            website: "Food Panda", description: string.Empty, onlineTransactionDetails: string.Empty, isPickup: isPickUp,
                            lat: order?.delivery?.address?.latitude ?? string.Empty, lon: order?.delivery?.address?.longitude ?? string.Empty, orderType: order?.expeditionType ?? string.Empty,
                            token: order?.token ?? string.Empty
                            );
                        long orderIdTemp = 0;
                        foreach (long n in orderNumberTemp)
                        {
                            orderIdTemp = n;
                        }
                        if (orderIdTemp != 0)
                        {
                            foreach (FPOrderItemsDetails i in itemsList)
                            {
                                
                                Menu menu = entity.Menus.FirstOrDefault((Menu e) => e.barcodeFP == i.BarCode);
                                if (menu != null)
                                {
                                    entity.Proc_SaveOrderDetailTemp_FP(orderIdTemp, menu.FamilyID, menu.MenuItemID, i.GrossPrice, i.Quantity, false, false, false, 
                                        menu.FamilyMeasureID, i.BarCode, i.Intruction);
                                }
                            }
                        }

                        if (hdsBranch != null && (hdsBranch.EnabledForFPAutoAcceptance ?? 0) == 1)
                        {
                            entity.Proc_ReceiveFoodPandaOrderOnBranch(deliveryOrderId: orderIdTemp);
                            //TAX SETTINGS IMPLEMENT HERE WHEN ACK METHOD CHANGED
                            performTaxActivity(branchId, orderIdTemp, order.payment.type, Convert.ToInt32(order.price.totalNet));
                            //TAX SETTINGS ENDED
                        }
                        else
                        {
                            SaveOrderAckQueue(order, orderIdTemp);
                        }
                        //entity.Proc_ReceiveFoodPandaOrderOnBranch(deliveryOrderId: orderIdTemp);
                        
                        //string nLine = Environment.NewLine;
                        //string r = "{" + nLine + "  \"remoteResponse\": {" + nLine + "    \"remoteOrderId\": \"POS_RESTAURANT_" + branchId + "_ORDER_" + orderIdTemp + "\"" + nLine + "  }" + nLine + "}";

                        //                      string str = $"{
                        //"remoteResponse": {
                        //                          "remoteOrderId": "POS_RESTAURANT_0001_ORDER_000001"
                        //}
                        //                  }
                        //                  "
                        //                      string orderDetail = @"{" + Environment.NewLine + "\"remoteOrderId\": \"POS_RESTAURANT_" + branchId + "_ORDER_" + orderIdTemp + "\"" + Environment.NewLine + "}";
                        //                      string response = @"{" + Environment.NewLine + "\"remoteResponse\": " + orderDetail + Environment.NewLine + "}";
                        string response = JsonConvert.SerializeObject(new SuccessResponse
                        {
                            remoteResponse =
                                {
                                    remoteOrderId = "POS_RESTAURANT_" + branchId + "_ORDER_" + orderIdTemp
                                }
                        });
                        UpdateOrderLoad(startTime, response, orderIdTemp);
                        return response;
                    }
                }
                else
                {
                    string failureResponse = JsonConvert.SerializeObject(new FailureResponse
                    {
                        reason = "MENU_ACCOUNT_SETTINGS",
                        message = valiationResponse
                    });
                    UpdateOrderLoad(startTime, failureResponse, -1);
                    return failureResponse;
                }
            }
            catch (Exception ex)
            {
                string exceptoionResponse = JsonConvert.SerializeObject(new FailureResponse
                {
                    reason = "TECHNICAL_PROBLEM",
                    message = ex.Message
                });
                UpdateOrderLoad(startTime, exceptoionResponse,-1);
                return exceptoionResponse;
            }
        }

        //private void RejectOrderDueToDupliction(string token)
        //{
        //    throw new NotImplementedException();
        //    //new RejectOrder(token);
        //}

        private string setPayMethod(FPPayment payment)
        {
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                foodPandaDeliveryBranch branch = entity.foodPandaDeliveryBranches.FirstOrDefault(e => e.branchId == hdsBranch.BranchID);
                if (branch != null)
                {
                    return "paid";
                }
            }
            return payment?.status ?? string.Empty;
        }

        private void UpdateOrderLoad(DateTime startTime, string response, long orderId)
        {
            DateTime endTime = DateTime.Now;
            double totalSeconds = (endTime - startTime).TotalSeconds;
            if (FoodPandaPaylaod == null || FoodPandaPaylaod.Id <= 0)
            {
                return;
            }
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                FoodPandaPayload val = entity.FoodPandaPayloads.FirstOrDefault(e => e.Id == FoodPandaPaylaod.Id);
                if (val != null)
                {
                    val.requestEndTime = endTime;
                    val.response = response;
                    val.totalDuration = totalSeconds.ToString();
                    val.requestStartTime = startTime;
                    val.orderNo = orderId.ToString();
                    entity.SaveChanges();
                }
            }
        }

        private void SaveOrderAckQueue(FPOrder fPOrder, long orderIdTemp)
        {
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                entity.FpAcknowledgementQueues.Add(
                    new FpAcknowledgementQueue()
                    {
                        cd = DateTime.Now,
                        DeliveryOrderIdTemp = orderIdTemp,
                        IsRead = 0,
                        nooftries = 0,
                        token = fPOrder?.token ?? string.Empty

                    }
                    );

                entity.SaveChanges();
            }
        }

        private void AcceptOrder(string token, long orderIdTemp)
        {
            new FPService().AcceptOrder(token, orderIdTemp.ToString());
        }

        private void RejectOrderDueToDupliction(string token)
        {
            new FPService().RejectOrder(token);
        }

        private int getDeliveryCharges(string delieryCharges)
        {
            if (delieryCharges.Contains("."))
            {
                return int.Parse(delieryCharges.Split('.')[0]);
            }
            return int.Parse(delieryCharges);
        }

        private string setProductItems(List<FPItemsList> q)
        {
            List<ProductItemCode> list = new List<ProductItemCode>();
            foreach (FPItemsList item in q)
            {
                list.Add(new ProductItemCode
                {
                    BarCode = item.remoteCode,
                    GrossPrice = item.unitPrice,
                    Quantity = item.quantity
                });
            }
            return JsonConvert.SerializeObject(list);
        }

        private bool CheckProducts(List<FPItemsList> products)
        {
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                ProductPrice = 0;
                foreach (FPItemsList item in products)
                {
                    int productQuantity = int.Parse(item.quantity);
                    if (!string.IsNullOrEmpty(item.comment))
                    {
                        Instructions = Instructions + " " + item.comment.Trim();
                    }
                    if (!string.IsNullOrEmpty(item.remoteCode) && productQuantity > 0)
                    {
                        int productPrice = !string.IsNullOrEmpty(item.unitPrice) ? int.Parse(item.unitPrice) : 0;
                        ProductPrice += productPrice * productQuantity;
                        if (entity.Menus.FirstOrDefault((Menu e) => e.barcodeFP == item.remoteCode && e.MenuPrice == (long?)(long)productPrice) == null)
                        {
                            return false;
                        }
                        foreach (FPToppings topping in item.selectedToppings)
                        {
                            int topingQuantity = int.Parse(topping.quantity);
                            int toppingPrice = !string.IsNullOrEmpty(topping.price) ? int.Parse(topping.price) : 0;
                            if (toppingPrice == 0)
                            {
                                Instructions = Instructions + " " + topping.name;
                                continue;
                            }
                            ProductPrice += toppingPrice * productQuantity;
                            // Following line is commented and replaced by above line to fix the topping quantity mismatch issue. 
                            // when this issue is solved by fp then following line should be uncommented and above line should be uncommented
                            //ProductPrice += toppingPrice * topingQuantity;
                            if (entity.Menus.FirstOrDefault((Menu e) => e.barcodeFP == topping.remoteCode && e.MenuPrice == toppingPrice) != null)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }

                        }

                    }
                    //return false;
                }
            }
            return true;
        }

        private List<FPOrderItemsDetails> getItemsList(List<FPItemsList> fpList)
        {
            List<FPOrderItemsDetails> list = new List<FPOrderItemsDetails>();
            foreach (FPItemsList fp in fpList)
            {
                string instruction = string.IsNullOrWhiteSpace(fp.comment) ? string.Empty : fp.comment;
                list.Add(new FPOrderItemsDetails
                {
                    BarCode = fp.remoteCode,
                    GrossPrice = int.Parse(fp.unitPrice),
                    Quantity = int.Parse(fp.quantity),
                    Intruction = instruction
                });
                foreach (FPToppings selectedTopping in fp.selectedToppings)
                {
                    if (int.Parse(selectedTopping.price) > 0)
                    {

                        list.Add(new FPOrderItemsDetails
                        {
                            BarCode = selectedTopping.remoteCode,
                            GrossPrice = int.Parse(selectedTopping.price),
                            Quantity = int.Parse(fp.quantity),
                            Intruction = string.Empty
                        });
                    }
                }
            }
            return list;
        }

        private long AddCustomerTemp(FPCustomer cust, string addess)
        {
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                string obj = (string.IsNullOrEmpty(cust.firstName) ? string.Empty : cust.firstName);
                string text = string.Concat(str2: (string.IsNullOrEmpty(cust.lastName) ? string.Empty : cust.lastName).Trim(), str0: obj.Trim(), str1: " ").Trim();
                string address = (string.IsNullOrEmpty(addess) ? string.Empty : addess);
                string firstName = text;
                string phone = (string.IsNullOrEmpty(cust.mobilePhone) ? string.Empty : cust.mobilePhone);
                string empty = string.Empty;
                string empty2 = string.Empty;
                string email = (string.IsNullOrEmpty(cust.email) ? string.Empty : cust.email.Trim());
                string empty3 = string.Empty;
                ObjectResult objectResult = entity.proc_InsertCustomerTemp(address, firstName, phone, empty, empty2, email, empty3);
                long result = 0;
                foreach (long item in objectResult)
                {
                    result = item;
                }
                return result;
            }
        }

        private string getDeliveryAddress(Address address)
        {
            string city = (string.IsNullOrEmpty(address?.city) ? string.Empty : (address.city ?? string.Empty));
            string street = (string.IsNullOrEmpty(address?.street) ? string.Empty : (address.street ?? string.Empty));
            string houseNumber = (string.IsNullOrEmpty(address?.number) ? string.Empty : (address.number ?? string.Empty));
            return (houseNumber + " " + street + " " + city).Trim();
        }

        public FPCancelationResponse UpdateOrderStatus(string branchCode, string orderId, CancelOrderPayload payLoad)
        {
            DateTime startTime = DateTime.Now;
            FPCancelationResponse fpResponse = new FPCancelationResponse();
            try
            {
                SaveFPOrderPayLoad(branchCode, payLoad, "Update Order", orderId);
                string cancellationReason = MapFPReasons(payLoad.message);
                using (KFC_PortalEntities entity = new KFC_PortalEntities())
                {
                    DateTime dt = DateTime.Now.AddMinutes(-5);
                    Branch hdsBranch = entity.Branches.FirstOrDefault(e => e.Branch_Code == branchCode);
                    if (hdsBranch != null)
                    {

                        long oId = long.Parse(orderId);
                        DeliveryOrder order = entity.DeliveryOrders.FirstOrDefault(e => e.BookedDateTime >= dt  && e.OrderThrough.ToLower().Trim() == "food panda" && e.DeliveryOrderIDTemp == oId && (e.OrderStatus == 1 || e.OrderStatus == 2));
                        if (order != null)
                        {
                            order.OrderStatus = 10;
                            order.CancelComment = cancellationReason;
                            order.CancelDateTime = DateTime.Now;
                            order.FinalStatusDateTIme = DateTime.Now;
                            order.CancelAgentID = 1834;
                            order.FinalStatusByID = 1834;
                            order.FPCancelationReason = payLoad.message;
                            entity.SaveChanges();

                            fpResponse.Reason = "Success";
                            fpResponse.Message = "Order Status updated successfully";
                            InsertPosEntry(order.DeliveryOrderID, 10, "Update order status");
                        }
                        else
                        {
                            foodPandaDeliveryBranch fpBranch = entity.foodPandaDeliveryBranches.FirstOrDefault(e => e.branchId == hdsBranch.BranchID);
                            if (fpBranch != null)
                            {
                                DateTime dt1 = DateTime.Now.AddMinutes(-120);
                                DeliveryOrder order2 = entity.DeliveryOrders.FirstOrDefault(e => e.BookedDateTime >= dt1 && e.DeliveryOrderIDTemp == oId && e.OrderThrough.ToLower().Trim().Equals("food panda") && (e.OrderStatus == 1 || e.OrderStatus == 2));
                                if (order2 != null)
                                {
                                    order2.OrderStatus = 10;
                                    order2.CancelComment = cancellationReason;
                                    order2.CancelDateTime = DateTime.Now;
                                    order2.FinalStatusDateTIme = DateTime.Now;
                                    order2.CancelAgentID = 1834;
                                    order2.FinalStatusByID = 1834;
                                    order2.FPCancelationReason = payLoad.message;
                                    order2.CancelByKFCMiddelWear = 1;
                                    entity.SaveChanges();
                                    InsertPosEntry(order2.DeliveryOrderID, 3, "Update order status");
                                }
                                fpResponse.Reason = "NOT_FOUND";
                                fpResponse.Message = "You can only cancel orders within five minutes after booking. Order delivered as Delivered in HDS";
                            }
                            else
                            {
                                fpResponse.Reason = "NOT_FOUND";
                                fpResponse.Message = "You can only cancel orders within five minutes after booking. Order delivered as Delivered in HDS";
                            }
                        }
                    }
                    else
                    {
                        fpResponse.Reason = "NOT_FOUND";
                        fpResponse.Message = $"Branch with {branchCode} does not exists";
                    }
                    #region Commented Code
                    //if (fpBranch !=  null)
                    //{
                    //    long oId = long.Parse(orderId);
                    //    DeliveryOrder order = entity.DeliveryOrders.FirstOrDefault(e => e.BookedDateTime >= dt && e.BranchID == hdsBranch.BranchID && e.OrderThrough.ToLower().Trim() == "food panda" && e.DeliveryOrderIDTemp == oId &&  ( e.OrderStatus == 1 || e.OrderStatus == 2));
                    //    if (order != null)
                    //    {
                    //        order.OrderStatus = 10;
                    //        order.CancelComment = cancellationReason;
                    //        order.CancelDateTime = DateTime.Now;
                    //        order.FinalStatusDateTIme = DateTime.Now;
                    //        order.CancelAgentID = 1834;
                    //        order.FPCancelationReason = payLoad.message;
                    //        entity.SaveChanges();

                    //        fpResponse.Reason = "Success";
                    //        fpResponse.Message = "Order Status updated successfully";
                    //        InsertPosEntry(order.DeliveryOrderID, 10, "Update order status");
                    //    }
                    //    else
                    //    {
                    //        DateTime dt1 = DateTime.Now.AddMinutes(-120);
                    //        DeliveryOrder order2 = entity.DeliveryOrders.FirstOrDefault(e => e.BookedDateTime >= dt1 && e.BranchID == hdsBranch.BranchID && e.DeliveryOrderIDTemp == oId && e.OrderThrough.ToLower().Trim().Equals("food panda") && (e.OrderStatus == 1 || e.OrderStatus == 2));
                    //        if (order2 != null)
                    //        {
                    //            order2.OrderStatus = 10;
                    //            order2.CancelComment = cancellationReason;
                    //            order2.CancelDateTime = DateTime.Now;
                    //            order2.FinalStatusDateTIme = DateTime.Now;
                    //            order2.CancelAgentID = 1834;
                    //            order2.FPCancelationReason = payLoad.message;
                    //            order2.CancelByKFCMiddelWear = 1;
                    //            entity.SaveChanges();
                    //            InsertPosEntry(order.DeliveryOrderID, 10, "Update order status");
                    //        }
                    //        fpResponse.Reason = "NOT_FOUND";
                    //        fpResponse.Message = "You can only cancel orders within five minutes after booking. Order delivered as Delivered in HDS";
                    //    }
                    //}
                   
                    //else
                    //{

                    //    fpResponse.Reason = "NOT_FOUND";
                    //    fpResponse.Message = "In case of Vendor Delivery order is not allowded to cancel";
                    //}
                    //}
                    
 #endregion
                    

                }
                UpdateOrderLoad(startTime, JsonConvert.SerializeObject(fpResponse), long.Parse(orderId));
                return fpResponse;
                    
                   
            }
            catch (Exception ex)
            {
                fpResponse.Reason = "INTERNAL_ERROR";
                fpResponse.Message = ex.Message; 
                UpdateOrderLoad(startTime, JsonConvert.SerializeObject(fpResponse), long.Parse(orderId));
                return fpResponse;
            }
        }

        string MapFPReasons(string fpReason)
        {
            string response = "Other/ foodpanda reason";
            switch (fpReason.Trim().ToUpper())
            {
                case "ADDRESS_INCOMPLETE_MISSTATED":
                    response = "Mention Wrong Address Demand In Out Of Delivery Area";
                    break;
                case "BAD_LOCATION":
                    response = "Mention Wrong Address Demand In Out Of Delivery Area";
                    break;
                case "BAD_WEATHER":
                    response = "Delivery Close";
                    break;
                case "BILLING_PROBLEM":
                    response = "Other/ foodpanda reason";
                    break;
                case "BLACKLISTED":
                    response = "Fake Order";
                    break;
                case "CARD_READER_NOT_AVAILABLE":
                    response = "Other/ foodpanda reason";
                    break;
                case "CLOSED":
                    response = "Delivery Close";
                    break;
                case "CONTENT_WRONG_MISLEADING":
                    response = "Other/ foodpanda reason";
                    break;
                case "COURIER_ACCIDENT":
                    response = "Other/ foodpanda reason";
                    break;
                case "COURIER_UNREACHABLE":
                    response = "Other/ foodpanda reason";
                    break;
                case "DELIVERY_ETA_TOO_LONG":
                    response = "Late Delivery";
                    break;
                case "DUPLICATE_ORDER":
                    response = "Double Order";
                    break;
                case "EXTRA_CHARGE_NEEDED":
                    response = "Other/ foodpanda reason";
                    break;
                case "FOOD_QUALITY_SPILLAGE":
                    response = "Other/ foodpanda reason";
                    break;
                case "FRAUD_PRANK":
                    response= "Fake Order";
                    break;
                case "NO_PICKER":
                    response = "Other/ foodpanda reason";
                    break;
                case "ITEM_UNAVAILABLE":
                    response = "Product Unavailable";
                    break;
                case "LATE_DELIVERY":
                    response = "Late Delivery";
                    break;
                case "MENU_ACCOUNT_SETTINGS":
                    response = "Other/ foodpanda reason";
                    break;
                case "MISTAKE_ERROR":
                    response = "Order Modification";
                    break;
                case "MOV_NOT_REACHED":
                    response = "Other/ foodpanda reason";
                    break;
                case "NEVER_DELIVERED":
                    response = "Late Delivery";
                    break;
                case "N0_COURIER":
                    response = "Rider Shrinkage";
                    break;
                case "NO_RESPONSE":
                    response = "Not Responding-Number Power OFF";
                    break;
                case "ONLINE_PAYMENT":
                    response = "Other/ foodpanda reason";
                    break;
                case "ORDER_MODIFICATION_NOT_POSSIBLE":
                    response = "Order Modification";
                    break;
                case "OUTSIDE_DELIVERY_AREA":
                    response = "Wrong Location Select-Order Send In Wrong Branch";
                    break;
                case "OUTSIDE_SERVICE_HOURS":
                    response = "Other/ foodpanda reason";
                    break;
                case "REASON_UNKNOWN":
                    response = "Other/ foodpanda reason";
                    break;
                case "TECHNICAL_PROBLEM":
                    response = "Other/ foodpanda reason";
                    break;
                case "TEST_ORDER":
                    response = "Other/ foodpanda reason";
                    break;
                case "TOO_BUSY":
                    response = "Other/ foodpanda reason";
                    break;
                case "UNABLE_TO_FIND":
                    response = "Not Responding-Number Power OFF";
                    break;
                case "UNABLE_TO_PAY":
                    response = "Wrong Order Punched-Wrong Payment Mode Select";
                    break;
                case "UNPROFESSIONAL_BEHAVIOUR":
                    response = "Other/ foodpanda reason";
                    break;
                case "UNREACHABLE":
                    response = "Other/ foodpanda reason";
                    break;
                case "VOUCHER_NOT_APPLIED":
                    response = "Other/ foodpanda reason";
                    break;
                case "WILL_NOT_WORK_WITH_PLATFORM":
                    response = "Other/ foodpanda reason";
                    break;
                case "WRONG_ORDER_ITEMS_DELIVERED":
                    response = "Other/ foodpanda reason";
                    break;

            }
            return response;
            
        }

        private void InsertPosEntry(long deliveryOrderId, int statusId, string apiName)
        {
            using (KFC_PortalEntities kFC_PortalEntities = new KFC_PortalEntities())
            {
                kFC_PortalEntities.Proc_addDynamicPosQueue(apiName, deliveryOrderId.ToString(), statusId.ToString(), string.Empty, string.Empty);
            }
        }

        private string ValidateFPOrder(FPOrder order)
        {
            string result = "Success";
            int deliveryCharges = int.Parse(ConfigurationManager.AppSettings["DeliveryCharges"].ToString());
            int orderDeliveryCharges = getDeliveryCharges(order?.price?.deliveryFee ?? string.Empty);
            string paymentMode = order?.payment?.status ?? string.Empty;
            if (IsFoodPandaDelivery())
            {
                if (orderDeliveryCharges == 0)
                {
                    orderDeliveryCharges = deliveryCharges;
                }
            }
            else if (orderDeliveryCharges == 0 && paymentMode.ToLower().Equals("paid"))
            {
                orderDeliveryCharges = deliveryCharges;
            }
            if (order != null && order.delivery != null && order.delivery.address != null && !string.IsNullOrEmpty(order.delivery.deliveryInstructions))
            {
                Instructions = Instructions + " " + order.delivery.deliveryInstructions.Trim();
            }
            if (order != null)
            {
                if (string.IsNullOrEmpty(order.expeditionType))
                {
                    result = "Invalid expedition type";
                }
                if (string.IsNullOrEmpty(order.code))
                {
                    result = "Invalid Order Code";
                }
                else if (validateOrderBranch() != "Success")
                {
                    result = "Invalide Branch Code";
                }
                if (string.IsNullOrEmpty(order.token))
                {
                    result = "Invalid token";
                }
                if (order.customer == null)
                {
                    result = "Customer Node is not available";
                }
                else if (!ValidateCustomer(order.customer))
                {
                    result = "Invalid Customer Data";
                }
                if (string.IsNullOrEmpty(order.expeditionType) && !order.expeditionType.Trim().ToLower().Equals("pickup") && order.delivery == null)
                {
                    result = "Delivery Node is not available";
                }
                else if (string.IsNullOrEmpty(order.expeditionType) && !order.expeditionType.Trim().ToLower().Equals("pickup") && !ValidateDeliveryAddress(order.delivery))
                {
                    result = "Invalid Delivery Address";
                }
                if (order.payment == null)
                {
                    result = "Payment Node is not available";
                }
                else if (!validatePayment(order.payment))
                {
                    result = "Invalid Payment ";
                }
                if (order.products == null)
                {
                    result = "Products Node is not available";
                }
                else if (!ValidateProducts(order.products))
                {
                    result = "Products with invalid remote id";
                }
                if (order.price == null)
                {
                    result = "Price Node is not available";
                }
                else if (!validatePrice(order.price))
                {
                    result = "Price mismatch with Products";
                }
                else if (orderDeliveryCharges != deliveryCharges)
                {
                    result = "Invalid Delivery Charges";
                }
            }
            return result;
        }

        private bool IsFoodPandaDelivery()
        {
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                Branch branch = entity.Branches.FirstOrDefault(e => e.Branch_Code == BranchId);
                if (branch != null)
                {
                    foodPandaDeliveryBranch fpBranch = entity.foodPandaDeliveryBranches.FirstOrDefault(e => e.branchId == branch.BranchID);
                    if (fpBranch != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private string validateOrderBranch()
        {
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                Branch branch = entity.Branches.FirstOrDefault((Branch e) => e.Branch_Code == BranchId);
                if (branch == null)
                {
                    return "Branch with Code : " + BranchId + " does not exists";
                }
                hdsBranch = branch;
            }
            return "Success";
        }

        private bool validatePayment(FPPayment payment)
        {
            if (string.IsNullOrEmpty(payment.remoteCode) || string.IsNullOrEmpty(payment.status) || string.IsNullOrEmpty(payment.type))
            {
                return false;
            }
            else if (!paymethod.Contains(payment.status.ToLower()))
            {
                return false;
            }
            return true;
        }

        private bool ValidateProducts(List<FPItemsList> products)
        {
            if (products != null && products.Count > 0 && CheckProducts(products))
            {
                return true;
            }
            return false;
        }

        private bool validatePrice(FPPrice price)
        {
            if (price != null && !string.IsNullOrEmpty(price.totalNet.Trim()) && double.Parse(price.totalNet) > 0.0 && int.Parse(price.totalNet) == ProductPrice)
            {
                return true;
            }
            return false;
        }

        private bool ValidateDeliveryAddress(DeliveryAddress delivery)
        {
            if (delivery != null && delivery.address != null && !string.IsNullOrEmpty(delivery.address.city.Trim()) && !string.IsNullOrEmpty(delivery.address.street.Trim()))
            {
                return true;
            }
            return false;
        }

        private bool ValidateCustomer(FPCustomer customer)
        {
            if (customer != null)
            {
                if (string.IsNullOrEmpty(customer.firstName.Trim()) && string.IsNullOrEmpty(customer.lastName.Trim()))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(customer.mobilePhone.Trim()))
                {
                    return false;
                }
                return true;
            }
            return false;
        }



        private string performTaxActivity(string branchId, long orderNo, string paymentMode, long OrderAmount)
        {

            long DeliveryOrderID = 0;

            //using (KFC_PortalEntities entity = new KFC_PortalEntities())
            //{
            //    DeliveryOrder getdeliveryOrder = entity.DeliveryOrders.FirstOrDefault((DeliveryOrder e) => e.DeliveryOrderIDTemp == orderNo);
            //    DeliveryOrderID = getdeliveryOrder.DeliveryOrderID;

            //}
            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                DeliveryOrder getdeliveryOrder = entity.DeliveryOrders.FirstOrDefault((DeliveryOrder e) => e.DeliveryOrderIDTemp == orderNo);

                if (getdeliveryOrder != null)
                {
                    DeliveryOrderID = getdeliveryOrder.DeliveryOrderID;
                    var branchDetails = entity.proc_getBranchDetailsbyIDforSimplex(id: Convert.ToInt32(branchId));
                    string POSID = "";
                    string Token = "";
                    string TaxAuthority = "";
                    double CreditTaxRate = 0;
                    double CashTaxRate = 0;
                    string PCTCode = "";
                    string isTaxEnabled = "";
                    string InvoiceType = "";


                    foreach (var item in branchDetails.ToList())
                    {
                        // Access the property and assign it to a string variable

                        isTaxEnabled = item.isTaxWorkingEnabled.ToString();

                        if (isTaxEnabled.ToLower() == "true")
                        {
                            POSID = item.POSID.ToString();
                            Token = item.TokenFBR;
                            TaxAuthority = item.TaxAuthority;
                            CreditTaxRate = Convert.ToInt64(item.CreditTaxRate);
                            CashTaxRate = Convert.ToInt64(item.CashTaxRate);
                            PCTCode = item.PCTCode;
                            InvoiceType = "1";// For New Order 1

                            // Do something with the stringValue

                            //DataTable OrderDetail = kfc.getOrderDetailDataTable(OrderID);
                            var OrderDetail = entity.Proc_getOrderDetail(orderID: DeliveryOrderID.ToString());
                            List<selectedMenuItemList2> selectedMenuItemListsNew = new List<selectedMenuItemList2>();

                            foreach (var itemDet in OrderDetail)
                            {
                                selectedMenuItemListsNew.Add(new selectedMenuItemList2()
                                {
                                    MenuItemID = itemDet.MenuItemID.ToString(),
                                    MenuItemText = itemDet.Title,
                                    Quantity = itemDet.Quantity.ToString(),
                                    GrossPrice = itemDet.MenuPrice.ToString(),
                                    DeliveryOrderDetailID = itemDet.DeliveryOrderDetailID.ToString()

                                });
                            }


                            get_FBRInvoice(OrderAmount.ToString(), "", paymentMode,
                  selectedMenuItemListsNew, DeliveryOrderID.ToString(), POSID, Token, TaxAuthority, CreditTaxRate, CashTaxRate, PCTCode, InvoiceType);

                        }
                        //else
                        //{

                        //}



                    }
                }

            }

            return "";

        }


        public static void get_FBRInvoice(string TotalBillAmount, string Servicecharges, string PaymentMode,
        List<selectedMenuItemList2> selectedMenuItemLists, string OrderID,
        string POSID, string Token, string TaxAuthority,
        double CreditTaxRate, double CashTaxRate, string PCTCode, string InvoiceType)
        {
            //FBRService fbr = new FBRService();
            //BusinessLayer.Bus_FBRInvoiceGen inv = new BusinessLayer.Bus_FBRInvoiceGen();
            int PaymentModeFBR = 0;
            double TaxRate = 0;
            string resFBR = "";
            double TotalAmountAddSC = 0;
            string finalResult = "";
            //TotalAmountAddSC = double.Parse(TotalBillAmount) + double.Parse(Servicecharges);
            bool isCredit = false;

            //TotalBillAmount (Inclusive of Tax)

            if (PaymentMode.ToLower() == "cash on delivery")
            {
                PaymentModeFBR = 1;

                TaxRate = CashTaxRate; //Can be Credit - Setting due
            }
            //else if (PaymentMode.ToLower() == "credit"
            //     || PaymentMode.ToLower() == "food panda online"
            //     || PaymentMode.ToLower() == "kfc web online"
            //     || PaymentMode.ToLower() == "keenu bank selection"
            //     || PaymentMode.ToLower() == "fp corporate order"
            //     || PaymentMode.ToLower() == "visa qr"
            //     || PaymentMode.ToLower() == "jazz cash online"
            //     || PaymentMode.ToLower() == "jazz cash wallet")
            //{
            //    PaymentModeFBR = 2;
            //    isCredit = true;
            //    TaxRate = CreditTaxRate; //Can be Credit - Setting due
            //}

            //string resFBR = fbr.TestFBR_POS("151858", double.Parse(Amount), TotalTaxCharged, PaymentModeFBR, InvoiceType, selectedMenuItemLists, OrderID);

            if (InvoiceType == "1")
            {

                resFBR = NewOrder_FBR_POS(POSID, double.Parse(TotalBillAmount),
                PaymentModeFBR, InvoiceType, selectedMenuItemLists,
                OrderID, Token, TaxAuthority, TaxRate, CashTaxRate, CreditTaxRate, isCredit);



                dynamic data = JObject.Parse(resFBR);
                string Response = data["Response"];
                if (Response == "Invoice received successfully")
                {
                    string InvoiceNumber = data["InvoiceNumber"];

                    using (KFC_PortalEntities entity = new KFC_PortalEntities())
                    //Save Invoice Number
                    {
                        entity.Proc_insertFBRInvoiceNumber(OrderID, InvoiceNumber, InvoiceType);
                    }
                    finalResult = "success";

                }
            }


            // return resFBR;
        }


        public static string NewOrder_FBR_POS(string POSID, double TotalBillAmount,
        int PaymentModeFBR, string InvoiceType, List<selectedMenuItemList2> selectedMenuItemLists,
        string OrderID, string Token, string TaxAuthority, double Taxrate, double CashTaxRate, double CreditTaxRate, bool isCredit)
        {

            //TotalBillAmount = TotalBillAmount - 50;
            TotalBillAmount = TotalBillAmount + 50;
            string URL_FBR = string.Empty;
            string URL_BRA = string.Empty;
            string URL_Test = string.Empty;

            using (KFC_PortalEntities entity = new KFC_PortalEntities())
            {
                var getAPIURL = entity.Proc_getAPI_forTax();

                foreach (var i in getAPIURL)
                {
                    URL_FBR = i.FBR_API_LINK.ToString();
                    URL_BRA = i.BRA_API_LINK.ToString();
                    URL_Test = i.TestLink.ToString();
                }


                double TotalSaleValue = 0;
                //double TotalBillAmountforFBR = 0;
                double TotalTaxCharged = 0;
                double TotalBillAmountMain = TotalBillAmount;
                double TotalDiscount = 0;

                if (isCredit == true)
                {
                    // Total AMount = 1000 
                    // Total Sale Value = 1000/1+0.17
                    //Total TAx Charged = Total AM - Total Sale Value

                    TotalSaleValue = TotalBillAmount / (1 + (CashTaxRate / 100)); // Exclusive of Tax
                    TotalTaxCharged = TotalBillAmount - TotalSaleValue; // 



                    TotalBillAmount = TotalSaleValue + (TotalSaleValue * (CreditTaxRate / 100)); // Exclusive of Tax
                    TotalTaxCharged = TotalBillAmount - TotalSaleValue; // 
                    TotalDiscount = TotalBillAmountMain - TotalBillAmount;

                }
                else
                {

                    TotalSaleValue = TotalBillAmount / (1 + (CashTaxRate / 100)); // Exclusive of Tax
                    TotalTaxCharged = TotalBillAmount - TotalSaleValue; // 


                }



                try
                {
                    var _contract = new Root();

                    List<ItemforFBR> info = new List<ItemforFBR>();


                    for (int i = 0; i < selectedMenuItemLists.Count; i++)
                    {

                        double ItemSaleValue = 0;
                        double ItemTaxCharged = 0;
                        double ItemTotalAmount = 0;
                        double ItemTotalAmountMain = 0;
                        double ItemTotalDiscount = 0;


                        if (isCredit == true)
                        {

                            ItemTotalAmount = int.Parse(selectedMenuItemLists[i].GrossPrice);
                            ItemTotalAmountMain = int.Parse(selectedMenuItemLists[i].GrossPrice);
                            ItemSaleValue = ItemTotalAmount / (1 + (CashTaxRate / 100)); // Exclusive of Tax
                            ItemTaxCharged = ItemTotalAmount - ItemSaleValue; // 
                            ItemTotalAmount = ItemSaleValue + (ItemSaleValue * (CreditTaxRate / 100)); // Exclusive of Tax
                                                                                                       // ItemTaxCharged = ItemTotalAmount - (int)Math.Ceiling(ItemSaleValue); // 
                            ItemTaxCharged = ItemTotalAmount - ItemSaleValue;
                            ItemTotalDiscount = ItemTotalAmountMain - ItemTotalAmount;

                            ItemTotalAmount = ItemTotalAmount * int.Parse(selectedMenuItemLists[i].Quantity);
                            ItemSaleValue = ItemSaleValue * int.Parse(selectedMenuItemLists[i].Quantity);
                            ItemTaxCharged = ItemTaxCharged * int.Parse(selectedMenuItemLists[i].Quantity);

                            entity.Proc_updateOrderDetailsforItemTaxDetails(Convert.ToInt32(selectedMenuItemLists[i].DeliveryOrderDetailID), Convert.ToInt32(Math.Round(ItemSaleValue, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(ItemTotalAmount, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(ItemTaxCharged, MidpointRounding.AwayFromZero)), Convert.ToInt32(CashTaxRate));



                        }
                        else
                        {

                            ItemTotalAmount = int.Parse(selectedMenuItemLists[i].GrossPrice);
                            //ItemTotalAmountMain = int.Parse(selectedMenuItemLists[i].GrossPrice);
                            ItemSaleValue = ItemTotalAmount / (1 + (CashTaxRate / 100)); // Exclusive of Tax
                            ItemTaxCharged = ItemTotalAmount - ItemSaleValue; // 
                            ItemTotalAmount = ItemTotalAmount * int.Parse(selectedMenuItemLists[i].Quantity);
                            ItemSaleValue = ItemSaleValue * int.Parse(selectedMenuItemLists[i].Quantity);
                            ItemTaxCharged = ItemTaxCharged * int.Parse(selectedMenuItemLists[i].Quantity);
                            try
                            {
                                entity.Proc_updateOrderDetailsforItemTaxDetails(Convert.ToInt32(selectedMenuItemLists[i].DeliveryOrderDetailID), Convert.ToInt32(Math.Round(ItemSaleValue, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(ItemTotalAmount, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(ItemTaxCharged, MidpointRounding.AwayFromZero)), Convert.ToInt32(CashTaxRate));

                            }
                            catch (Exception ex)
                            {
                                ex.InnerException.ToString();
                            }


                        }

                        info.Add(new ItemforFBR()
                        {
                            ItemCode = selectedMenuItemLists[i].MenuItemID,
                            ItemName = selectedMenuItemLists[i].MenuItemText,
                            PCTCode = "00000000", //MAY BE ADD IN ITEMS SETTINGS
                            Quantity = int.Parse(selectedMenuItemLists[i].Quantity),
                            TaxRate = Taxrate,//MAY BE ADD IN ITEMS SETTINGS
                            SaleValue = Math.Round(ItemSaleValue, MidpointRounding.AwayFromZero),
                            TaxCharged = Math.Round(ItemTaxCharged, MidpointRounding.AwayFromZero),
                            TotalAmount = Math.Round(ItemTotalAmount, MidpointRounding.AwayFromZero),
                            InvoiceType = int.Parse(InvoiceType) //MAY BE ADD IN ITEMS SETTINGS
                        });

                        //_contract._contract.items[i].ItemCode= selectedMenuItemLists[i].MenuItemID;

                        //_contract._contract.items[i].ItemName = selectedMenuItemLists[i].MenuItemText;
                        //_contract._contract.items[i].PCTCode = "00000000"; //MAY BE ADD IN ITEMS SETTINGS
                        //_contract._contract.items[i].Quantity = int.Parse(selectedMenuItemLists[i].Quantity);
                        //_contract._contract.items[i].TaxRate = 1; //MAY BE ADD IN ITEMS SETTINGS
                        //_contract._contract.items[i].SaleValue = int.Parse(selectedMenuItemLists[i].GrossPrice);
                        //_contract._contract.items[i].TaxCharged = 1;
                        //_contract._contract.items[i].TotalAmount = 1;
                        //_contract._contract.items[i].InvoiceType = int.Parse(InvoiceType); //MAY BE ADD IN ITEMS SETTINGS


                    }


                    //Delivery Charges As Item
                    double ItemSaleValueD = 0;
                    double ItemTaxChargedD = 0;
                    double ItemTotalAmountD = 0;
                    double ItemTotalAmountMainD = 0;
                    double ItemTotalDiscountD = 0;

                    if (isCredit == true)
                    {

                        ItemTotalAmountD = 50;
                        ItemTotalAmountMainD = 50;

                        ItemSaleValueD = ItemTotalAmountD / (1 + (CashTaxRate / 100)); // Exclusive of Tax
                        ItemTaxChargedD = ItemTotalAmountD - ItemSaleValueD; // 

                        ItemTotalAmountD = ItemSaleValueD + (ItemSaleValueD * (CreditTaxRate / 100)); // Exclusive of Tax
                                                                                                      // ItemTaxCharged = ItemTotalAmount - (int)Math.Ceiling(ItemSaleValue); // 
                        ItemTaxChargedD = ItemTotalAmountD - ItemSaleValueD;
                        ItemTotalDiscountD = ItemTotalAmountMainD - ItemTotalAmountD;

                    }
                    else
                    {

                        ItemTotalAmountD = 50;
                        //ItemTotalAmountMain = int.Parse(selectedMenuItemLists[i].GrossPrice);

                        ItemSaleValueD = ItemTotalAmountD / (1 + (CashTaxRate / 100)); // Exclusive of Tax
                        ItemTaxChargedD = ItemTotalAmountD - ItemSaleValueD; // 


                    }

                    info.Add(new ItemforFBR()
                    {
                        ItemCode = "ITM-3730",
                        ItemName = "Delivery Charges (N) Rs. 50",
                        PCTCode = "00000000", //MAY BE ADD IN ITEMS SETTINGS
                        Quantity = 1,
                        TaxRate = Taxrate,//MAY BE ADD IN ITEMS SETTINGS
                        SaleValue = Math.Round(ItemSaleValueD, MidpointRounding.AwayFromZero),
                        TaxCharged = Math.Round(ItemTaxChargedD, MidpointRounding.AwayFromZero),
                        TotalAmount = Math.Round(TotalSaleValue, MidpointRounding.AwayFromZero),
                        InvoiceType = int.Parse(InvoiceType) //MAY BE ADD IN ITEMS SETTINGS
                    });
                    //Delivery Charges As Item

                    _contract.POSID = int.Parse(POSID);
                    _contract.USIN = OrderID;
                    _contract.DateTime = DateTime.Now.ToString();
                    _contract.TotalSaleValue = Math.Round(TotalSaleValue, MidpointRounding.AwayFromZero);
                    _contract.TotalTaxCharged = Math.Round(TotalTaxCharged, MidpointRounding.AwayFromZero);
                    _contract.TotalBillAmount = Math.Round(TotalBillAmount, MidpointRounding.AwayFromZero);
                    _contract.PaymentMode = PaymentModeFBR;
                    _contract.TotalQuantity = selectedMenuItemLists.Count;
                    _contract.InvoiceType = int.Parse(InvoiceType);
                    _contract.items = info;
                    _contract.BuyerName = null;


                    ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback
                    (
                    delegate { return true; }
                    );
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;


                    using (WebClient webClient = new WebClient())
                    {

                        webClient.Proxy = new WebProxy();
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                        //webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer 1298b5eb-b252-3d97-8622-a4a69d5bf818");
                        webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
                        string data = JsonConvert.SerializeObject(_contract);

                        //obj.RequestJson = data;
                        string response = string.Empty;
                        //response = webClient.UploadString("https://esp.fbr.gov.pk:8244/FBR/v1/api/Live/PostData", data);
                        //response = webClient.UploadString(URL_Test, data);
                        if (TaxAuthority == "FBR")
                        {
                            //response = webClient.UploadString("https://gw.fbr.gov.pk/imsp/v1/api/Live/PostData", data);
                            response = webClient.UploadString(URL_FBR, data);
                        }
                        else if (TaxAuthority == "BRA")
                        {
                            //response = webClient.UploadString("https://ims.pral.com.pk/ims/production/api/Live/PostData", data);
                            response = webClient.UploadString(URL_BRA, data);
                        }


                        var result = JsonConvert.DeserializeObject<object>(response);
                        entity.Proc_insertFBRActivityLog(data, result.ToString());
                        //saveFBRActivityLog(data, result.ToString(), DateTime.Now.ToShortDateString());
                        if (result.ToString().Contains("Invoice received successfully"))
                        {
                            entity.Proc_insertFBRNewOrderDetails(Convert.ToInt32(OrderID), Convert.ToInt32(Taxrate), Convert.ToInt32(TotalBillAmount), Convert.ToInt32(TotalSaleValue), Convert.ToInt32(TotalDiscount), Convert.ToInt32(TotalTaxCharged), Convert.ToInt32(Math.Round(ItemTotalAmountD, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(ItemSaleValueD, MidpointRounding.AwayFromZero)), Convert.ToInt32(Math.Round(ItemTaxChargedD, MidpointRounding.AwayFromZero)));
                            //saveFBRFBRNewOrderDetails(OrderID, Taxrate, TotalBillAmount, TotalSaleValue, TotalDiscount, TotalTaxCharged, Math.Round(ItemTotalAmountD, MidpointRounding.AwayFromZero), Math.Round(ItemSaleValueD, MidpointRounding.AwayFromZero), Math.Round(ItemTaxChargedD, MidpointRounding.AwayFromZero));
                        }
                        return result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

            }


        }

    }
}
