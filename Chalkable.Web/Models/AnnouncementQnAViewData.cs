using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class AnnouncementQnAViewData
    {
        public Guid Id { get; set; }
        public Guid AnnouncementId { get; set; }
        public AnnouncementMessageViewData Question { get; set; }
        public AnnouncementMessageViewData Answer { get; set; }
        public bool IsOwner { get; set; }
        public int State { get; set; }
        public static AnnouncementQnAViewData Create(AnnouncementQnAComplex announcementQnA)
        {
            return new AnnouncementQnAViewData
            {
                Id = announcementQnA.Id,
                AnnouncementId = announcementQnA.AnnouncementRef,
                IsOwner = announcementQnA.IsOwner,
                Question = AnnouncementMessageViewData.Create(announcementQnA, false),
                Answer = AnnouncementMessageViewData.Create(announcementQnA, true),
                State = (int)announcementQnA.State
            };
        }
        public static IList<AnnouncementQnAViewData> Create(IList<AnnouncementQnAComplex> announcementQnAs)
        {
            return announcementQnAs.Select(Create).ToList();
        }
    }

    public class AnnouncementMessageViewData
    {
        public ShortPersonViewData Person { get; set; }
        public DateTime? Created { get; set; }
        public string Message { get; set; }

        public static AnnouncementMessageViewData Create(AnnouncementQnAComplex announcementQnA, bool isAnswerer)
        {
            if (isAnswerer)
            {
                return new AnnouncementMessageViewData
                {
                    Person = ShortPersonViewData.Create(announcementQnA.Answerer),
                    Message = announcementQnA.Answer,
                    Created = announcementQnA.AnsweredTime
                };
            }
            return new AnnouncementMessageViewData
            {
                Person = ShortPersonViewData.Create(announcementQnA.Asker),
                Message = announcementQnA.Question,
                Created = announcementQnA.QuestionTime
            };
        }
    }
}