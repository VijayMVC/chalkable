using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolOptionViewData
    {
        public bool AllowScoreEntryForUnexcused { get; set; }

        public static SchoolOptionViewData Create(SchoolOption schoolOption)
        {
            return new SchoolOptionViewData
                {
                    AllowScoreEntryForUnexcused = schoolOption.AllowScoreEntryForUnexcused
                };
        }
    }
}