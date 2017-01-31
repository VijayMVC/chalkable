using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class LimitedEnglishViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SifCode { get; set; }
        public string NcesCode { get; set; }
        public static IList<LimitedEnglishViewData> Create(IList<LimitedEnglish> limitedEnglishes)
        {
            return limitedEnglishes.Select(x => new LimitedEnglishViewData
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                StateCode = x.StateCode,
                NcesCode = x.NcesCode,
                SifCode = x.SifCode
            }).ToList();
        } 
    }
}