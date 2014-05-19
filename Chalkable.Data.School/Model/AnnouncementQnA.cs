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
        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int AskerRef { get; set; }
        public int? AnswererRef { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public AnnouncementQnAState State { get; set; }
        public DateTime? AnsweredTime { get; set; }
        public DateTime QuestionTime { get; set; }

        //[NotDbFieldAttr]
        //public Announcement Announcement { get; set; }

    }

    public class AnnouncementQnAComplex : AnnouncementQnA
    {
        public int ClassRef { get; set; }
        private Person asker;
        public Person Asker
        {
            get { return asker; }
            set
            {
                asker = value;
                if (AskerRef == 0 && asker != null)
                    AskerRef = asker.Id;
            }
        }
        private Person answerer { get; set; }
        public Person Answerer
        {
            get { return answerer; } 
            set
            {
                answerer = value;
                if (!AnswererRef.HasValue && answerer != null)
                    AnswererRef = answerer.Id;
            }
        }
        public bool IsOwner { get; set; }
    }


    public enum AnnouncementQnAState
    {
        Asked = 0,
        Answered = 1,
        Unanswered = 2
    }
}
