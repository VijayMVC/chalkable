using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models
{
    public class GradingStyleVeiwData
    {
        public IList<int> GradingAbcf { get; set; }
        public IList<int> GradingComplete { get; set; }
        public IList<int> GradingCheck { get; set; }
 
        private GradingStyleVeiwData()
        {
      
        }

        public static  GradingStyleVeiwData Create(IList<int> gradingAbcf, IList<int> gradingComlete, IList<int> gradingCheck)
        {
            var res = new GradingStyleVeiwData()
                          {
                              GradingAbcf = gradingAbcf,
                              GradingCheck = gradingCheck,
                              GradingComplete = gradingComlete

                          };

            return res;
        }
    }

    
    
}