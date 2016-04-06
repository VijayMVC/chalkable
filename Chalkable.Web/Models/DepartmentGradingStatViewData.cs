using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class DepartmentGradingStatViewData
    {
        public Guid DepartmentId { get; set; }
        public string Title { get; set; }
        public int? AvgByGradeLevels { get; set; }
        public int? FullAvg { get; set; }
        
        public static IList<DepartmentGradingStatViewData> Create(IList<ChalkableDepartment> departments,
            IList<DepartmentGradeAvg> statsByGl, IList<DepartmentGradeAvg> statsByAllGl)
        {
            var res = new List<DepartmentGradingStatViewData>();
            if (departments.Count > 0)
            {
                foreach (var department in departments)
                {
                    var avgByAllGl = statsByAllGl.FirstOrDefault(x => x.ChalkableDepartmentRef == department.Id);
                    var avgByGl = statsByGl.FirstOrDefault(x => x.ChalkableDepartmentRef == department.Id);
                    res.Add(new DepartmentGradingStatViewData
                    {
                        DepartmentId = department.Id,
                        AvgByGradeLevels = avgByGl != null ? avgByGl.Avg : null,
                        FullAvg = avgByAllGl != null ? avgByAllGl.Avg : null,
                        Title = department.Name
                    });
                }
            }
            return res;
        }
    }
}