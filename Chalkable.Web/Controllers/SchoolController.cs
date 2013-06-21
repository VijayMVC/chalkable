using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chalkable.Web.Controllers
{
    public class SchoolController : ChalkableController
    {

        private class StubSchoolInfoViewData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string LocalId { get; set; }
            public string NCESId { get; set; }
            public string SchoolType { get; set; }
            public string SchoolUrl { get; set; }
            public string TimeZoneId { get; set; }
            public bool SendEmailNotifications { get; set; }
        }

        public ActionResult List(int? start, int? count, bool? demoOnly)
        {
            count = count ?? 10;
            start = start ?? 0;


            var stubSchools = new List<StubSchoolInfoViewData>();

            stubSchools.Add(new StubSchoolInfoViewData
            {
                Id = 96,
                LocalId = null,
                Name = "Demo School",
                NCESId = null,
                SchoolType = null,
                SchoolUrl = null,
                SendEmailNotifications = false,
                TimeZoneId = "US Eastern Standard Time"
            });



           
            return Json(stubSchools);
        }

    }
}
