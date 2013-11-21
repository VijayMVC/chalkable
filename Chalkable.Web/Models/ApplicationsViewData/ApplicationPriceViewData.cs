using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationPriceViewData
    {
        public decimal Price { get; set; }
        public decimal? PricePerClass { get; set; }
        public decimal? PricePerSchool { get; set; }

        protected ApplicationPriceViewData() { }
        protected ApplicationPriceViewData(Application application)
        {
            Price = application.Price;
            PricePerClass = application.PricePerClass;
            PricePerSchool = application.PricePerSchool;

        }
        public static ApplicationPriceViewData Create(Application application)
        {
            return new ApplicationPriceViewData(application);
        }
    }
}