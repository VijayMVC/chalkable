using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models
{
    public class GradingStyleViewData
    {
        public IList<int> GradingAbcf { get; set; }
        public IList<int> GradingComplete { get; set; }
        public IList<int> GradingCheck { get; set; }
 
        private GradingStyleViewData()
        {
        }

        public static  GradingStyleViewData Create(IList<int> gradingAbcf, IList<int> gradingComlete, IList<int> gradingCheck)
        {
            var res = new GradingStyleViewData()
                          {
                              GradingAbcf = gradingAbcf,
                              GradingCheck = gradingCheck,
                              GradingComplete = gradingComlete

                          };

            return res;
        }
    }

    
    
}