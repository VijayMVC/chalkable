using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementQnA
    {
        public Guid Id { get; set; }
        public Guid AnnouncementRef { get; set; }
        public Guid PersonRef { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public AnnouncementQnAState State { get; set; }
        public DateTime AnsweredTime { get; set; }
        public DateTime QuestionTime { get; set; }

        //[NotDbFieldAttr]
        //public Announcement Announcement { get; set; }

    }

    public class AnnouncementQnAComplex : AnnouncementQnA
    {
        private Person asker;
        public Person Asker
        {
            get { return asker; }
            set
            {
                asker = value;
                if (PersonRef == Guid.Empty && asker != null)
                    PersonRef = asker.Id;
            }
        }
        public Person Answerer { get; set; }
        public bool IsOwner { get; set; }        
    }


    public enum AnnouncementQnAState
    {
        Asked = 0,
        Answered = 1,
        Unanswered = 2
    }
}
