using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class EthnicityViewData
    {
        public short Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static EthnicityViewData Create(Ethnicity ethnicity)
        {
            return ethnicity != null ? new EthnicityViewData
            {
                Id = ethnicity.Id,
                Code = ethnicity.Code,
                Name = ethnicity.Name,
                Description = ethnicity.Description
            } : null;
        }
    }
}