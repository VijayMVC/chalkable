using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Authority
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static Authority Create(AcademicBenchmarkConnector.Models.Authority a)
        {
            return new Authority
            {
                Id = a.Id,
                Code = a.Code,
                Description = a.Description
            };
        }
    }
}
