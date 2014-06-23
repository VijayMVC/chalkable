using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationTotalPriceViewData : ApplicationPriceViewData
    {
        public decimal TotalPrice { get; set; }
        public int TotalPersonsCount { get; set; }

        private ApplicationTotalPriceViewData(Application application, ApplicationTotalPriceInfo priceInfo): base(application)
        {
            TotalPrice = priceInfo.TotalPrice;
            TotalPersonsCount = priceInfo.TotalCount;
                // priceInfo.ApplicationInstallCountInfo.FirstOrDefault(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count.Value;
        }
        public static ApplicationTotalPriceViewData Create(Application application, ApplicationTotalPriceInfo priceInfo)
        {
            return new ApplicationTotalPriceViewData(application, priceInfo);
        }
    }
}