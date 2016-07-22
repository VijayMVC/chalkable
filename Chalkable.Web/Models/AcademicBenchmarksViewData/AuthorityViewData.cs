using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class AuthorityViewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static AuthorityViewData Create(Authority authority)
        {
            return new AuthorityViewData
            {
                Id = authority.Id,
                Code = authority.Code,
                Description = authority.Description
            };
        }
    }
}