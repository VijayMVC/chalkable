using System.Collections.Generic;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolInfoViewData : SchoolViewData
    {
        public IList<string> Emails { get; set; }
        public int[] Buttons { get; set; }

        protected SchoolInfoViewData(School school) : base(school)
        {
            Buttons = new[]{1, 2, 3, 4};
        }

        new public static SchoolInfoViewData Create(School school)
        {
            return new SchoolInfoViewData(school);
        }
    }
}