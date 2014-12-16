using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassAttendanceSummaryViewData : ClassViewData
    {
        //TODO: add HoverBoxes for attendanceInfo

        protected ClassAttendanceSummaryViewData(ClassDetails classDetails)
            : base(classDetails)
        {   
        }
        public new static ClassAttendanceSummaryViewData Create(ClassDetails classDetails)
        {
            return new ClassAttendanceSummaryViewData(classDetails);
        }
    }
}