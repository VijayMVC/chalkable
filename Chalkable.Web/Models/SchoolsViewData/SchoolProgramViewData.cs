using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolProgramViewData
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }

        protected SchoolProgramViewData(SchoolProgram schoolProgram)
        {
            Id = schoolProgram.Id;
            Code = schoolProgram.Code;
            Name = schoolProgram.Name;
            Description = schoolProgram.Description;
            StateCode = schoolProgram.StateCode;
            SIFCode = schoolProgram.SIFCode;
            NCESCode = schoolProgram.NCESCode;
            IsActive = schoolProgram.IsActive;
            IsSystem = schoolProgram.IsSystem;
        }

        public static SchoolProgramViewData Create(SchoolProgram schoolProgram)
        {
            return new SchoolProgramViewData(schoolProgram);
        }
        public static IList<SchoolProgramViewData> Create(IList<SchoolProgram> schoolPrograms)
        {
            return schoolPrograms.Select(Create).ToList();
        }
    }
}