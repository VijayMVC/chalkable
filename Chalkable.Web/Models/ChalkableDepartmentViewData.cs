using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class ChalkableDepartmentViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<string> Keywords { get; set; }

        private ChalkableDepartmentViewData(){ }
 
        public static ChalkableDepartmentViewData Create(ChalkableDepartment chalkableDepartment)
        {
            var sep = new [] {','};
            var res = new ChalkableDepartmentViewData
                          {
                              Id = chalkableDepartment.Id,
                              Name = chalkableDepartment.Name,
                              Keywords = chalkableDepartment.Keywords.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList(),
                          };
            return res;
        }

        public static IList<ChalkableDepartmentViewData> Create(IList<ChalkableDepartment> chalkableDepartments)
        {
            var res = chalkableDepartments.Select(Create);
            return res.ToList();
        }
    }
}