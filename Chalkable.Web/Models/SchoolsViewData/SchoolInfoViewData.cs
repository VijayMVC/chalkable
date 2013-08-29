using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolInfoViewData : SchoolViewData
    {
        public IList<string> Emails { get; set; }
        public int[] Buttons { get; set; }
        public SisSyncViewData SisData { get; set; }

        protected SchoolInfoViewData(School school, SisSync sisData) : base(school)
        {
            Buttons = GetButtons(school.Status);
            SisData = SisSyncViewData.Create(sisData);
        }

        public static SchoolInfoViewData Create(School school, SisSync sisData)
        {
            return new SchoolInfoViewData(school, sisData);
        }

        private static int[] GetButtons(SchoolStatus status)
        {
            return statusBtDic.ContainsKey(status) ? statusBtDic[status] : null;
        }
        private static IDictionary<SchoolStatus, int[]> statusBtDic = new Dictionary<SchoolStatus, int[]>
            {
                {SchoolStatus.PersonalInfoImported, new[] { 2, 1 }},
                {SchoolStatus.Created, new[] { 1 }},
                {SchoolStatus.DailyPeriods, new[] { 3, 2, 1 }},
                {SchoolStatus.StudentLogged, new[] { 4, 3, 2, 1 }},
                {SchoolStatus.PayingCustomer,  new[] { 1 , 2 }}
            }; 
    }
}