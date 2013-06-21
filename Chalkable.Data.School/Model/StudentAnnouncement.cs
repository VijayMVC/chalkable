﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public enum StateEnum
    {
        Auto = 0,
        None = 1,
        Manual = 2
    }

    public class StudentAnnouncement
    {
        public Guid Id { get; set; }
        public Guid AnnouncementRef { get; set; }
        public Guid PersonRef { get; set; }
        public int? GradeValue { get; set; }
        public string Comment { get; set; }
        public string ExtraCredit { get; set; }
        public bool Droppped { get; set; }
        public StateEnum State { get; set; }
        public Guid? ApplicationRef { get; set; }
    }

    public class StudentAnnouncementDetails : StudentAnnouncement
    {
        public Guid ClassId { get; set; }
        public Person Person { get; set; }
    }
}
