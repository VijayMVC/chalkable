using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class StudentAnnouncementApplicationMetaViewData
    {
        public int AnnouncementApplicationId { get; set; }
        public int StudentId { get; set; }
        public string Text { get; set; }

        protected StudentAnnouncementApplicationMetaViewData(StudentAnnouncementApplicationMeta studentAnnouncementApplicationMeta)
        {
            AnnouncementApplicationId = studentAnnouncementApplicationMeta.AnnouncementApplicationRef;
            StudentId = studentAnnouncementApplicationMeta.StudentRef;
            Text = studentAnnouncementApplicationMeta.Text;
        }

        public static IList<StudentAnnouncementApplicationMetaViewData> Create(IList<StudentAnnouncementApplicationMeta> studentAnnouncementApplicationMeta)
        {
            return studentAnnouncementApplicationMeta.Select(x => new StudentAnnouncementApplicationMetaViewData(x)).ToList();
        }
    }
}