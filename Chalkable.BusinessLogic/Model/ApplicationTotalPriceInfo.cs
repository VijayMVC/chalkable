using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ApplicationTotalPriceInfo
    {
        public decimal TotalPrice { get; set; }
        public IList<PersonsForApplicationInstallCount> ApplicationInstallCountInfo { get; set; }

        public static ApplicationTotalPriceInfo Create(decimal totalPrice, IList<PersonsForApplicationInstallCount> applicationInstallCount)
        {
            return new ApplicationTotalPriceInfo
                {
                    TotalPrice = totalPrice,
                    ApplicationInstallCountInfo = applicationInstallCount
                };
        }
    }
}