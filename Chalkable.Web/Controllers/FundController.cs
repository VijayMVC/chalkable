using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using PayPal;
using PayPal.Api.Payments;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FundController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult GetAppBudgetBalance()
        {
            return FakeJson("~/fakeData/appBudget.json");
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var schoolId = Context.SchoolId.Value;
            //var toSchoolPayment = (double)MasterLocator.FundService.GetToSchoolPayment(schoolId);
            //var paymentforApp = (double)MasterLocator.FundService.GetPaymentForApps(schoolId);
            //return Json(BudgetBalanceViewData.Craete(toSchoolPayment, paymentforApp));
        }

        //TODO: implementation for those methods 
        [AuthorizationFilter("Teacher, Student, AdminGrade, AdminEdit, AdminView")]
        public ActionResult GetPersonBudgetBalance(int personId)
        {
            var userBalance = (decimal)0;
            if (DemoUserService.IsDemoUser(Context))
            {
                userBalance = MasterLocator.FundService.GetUserBalance(personId);
            }
            return Json(new {balance = userBalance});
        }

        [AuthorizationFilter("Teacher, Student, Parent")]
        public ActionResult PersonFunds()
        {
            return FakeJson("~/fakeData/personFunds.json");
        }

        [AuthorizationFilter("Teacher, Student, Parent")]
        public ActionResult AddCredit(decimal amount, string cardNumber,int month,int year, int cvcNumber, string cardType)
        {
            var res = cardNumber.Replace(" ", "") != "1324982345234523";
            return Json(res);
        }

        private const string ACCESS_TOKEN_PARAM = "paypal_accessToken";
        private const string APPROVAL_URL_PARAM = "approval_url";
        private const string PAYMENT_ID_PARAM = "payment_id";
        private const string PAYPAL_PAYMENT_METHOD = "paypal";
        private const string URL_REFERRER_PARAM = "url_referrer";

        //TODO: refactor funding
        [AuthorizationFilter("Teacher, Student, Parent")]
        public ActionResult AddViaPayPal(decimal amount)
        {
            var accessToken = GetAccessToken();
            System.Web.HttpContext.Current.Response.Cookies.Set(new HttpCookie(ACCESS_TOKEN_PARAM)
                {
                    Value = accessToken,
                    Expires = Context.NowSchoolTime.AddDays(2)
                });
            System.Web.HttpContext.Current.Response.Cookies.Set(new HttpCookie(URL_REFERRER_PARAM, Request.UrlReferrer.AbsoluteUri));   
            var returnUrl = GetBaseAppUrlReferrer() + "Fund/ExecutePayPalPayment.json";
            var cancelUrl = string.Format("{0}#{1}", Request.UrlReferrer.AbsoluteUri, UrlsConstants.FUNDS_URL);
            var payment = CreatePayment(amount, returnUrl, cancelUrl, accessToken);
            var approvalUrl = payment.links.First(x => x.rel == APPROVAL_URL_PARAM);
            System.Web.HttpContext.Current.Response.Cookies.Set(new HttpCookie(PAYMENT_ID_PARAM, payment.id));
            return Json(new { RedirectUrl = approvalUrl.href });
        }

        public ActionResult ExecutePayPalPayment(string token, string PayerID)
        {
            var paymentEx = new PaymentExecution {payer_id = PayerID};
            var accessToken = System.Web.HttpContext.Current.Request.Cookies.Get(ACCESS_TOKEN_PARAM);
            if (accessToken == null || string.IsNullOrEmpty(accessToken.Value))
                return Json(new ChalkableException("There are no accessToken"));
            var apiContex = GetApiContext(accessToken.Value);
            var paymentId = System.Web.HttpContext.Current.Request.Cookies.Get(PAYMENT_ID_PARAM).Value;
            var payment = new Payment
                {
                    id = paymentId, 
                    intent = "update",
                    payer = new Payer { payment_method = PAYPAL_PAYMENT_METHOD }
                }.Execute(apiContex, paymentEx);
            var isCompleted = payment.transactions.Last().related_resources.First().sale.state == "completed";
            var urlReferrer = System.Web.HttpContext.Current.Request.Cookies.Get(URL_REFERRER_PARAM).Value;
            return Redirect(string.Format("{0}#{1}/{2}", urlReferrer, UrlsConstants.FUNDS_URL, isCompleted.ToString().ToLower()));
        }


        private static Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl, string accessToken)
        {
            var payer = new Payer { payment_method = PAYPAL_PAYMENT_METHOD };
            var amnt = new Amount { currency = "USD", total = amount.ToString() };
            var tran = new Transaction
                {
                    amount = amnt,
                    description = "This is the payment transaction description."
                };
            var payment = new Payment
                {
                    intent = "sale",
                    transactions = new List<Transaction> {tran},
                    payer = payer,
                    redirect_urls = new RedirectUrls
                        {
                            cancel_url = cancelUrl, 
                            return_url = returnUrl
                        }
                };
            return payment.Create(GetApiContext(accessToken)); 
        }

        private static Dictionary<string, string> GetConfig()
        {
            var configMap = new Dictionary<string, string>();
            // Endpoints are varied depending on whether sandbox OR live is chosen for mode
            configMap.Add("mode", "sandbox");
            // These values are defaulted in SDK. If you want to override default values, uncomment it and add your value
            // configMap.Add("connectionTimeout", "360000");
            // configMap.Add("requestRetries", "1");
            return configMap;
        }

        private static string GetAccessToken()
        {         
            string client_Id = "AfX6AxDokm5RBqOt4J1E8a3iPC2SukvKPhqJreZ_ED6AVh-lDzRCBN0S0b0w";
            string client_secret = "EO1FrBB8R-_Tx0TF_ox_QugmF1PbTFVERtbHHgrYvdWod28GDGdOWL_S4Plt";
            string accessToken = new OAuthTokenCredential(client_Id, client_secret, GetConfig()).GetAccessToken();
            return accessToken;
        }
      
        private static APIContext GetApiContext(string accessToken)
        {
            return new APIContext(accessToken) { Config = GetConfig() };
        }
        
        [AuthorizationFilter("Teacher, Student, Parent")]
        public ActionResult GetCreditCardInfo(bool needCardInfo)
        {
            return needCardInfo ? FakeJson("~/fakeData/creditCardData.json") : Json(null);
        }

        [AuthorizationFilter("Teacher, Student, Parent")]
        public ActionResult DeleteCreditCardInfo()
        {
            return Json(true);
        }
    }
}