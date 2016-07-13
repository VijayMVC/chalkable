﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class AnnouncementCommentViewData
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public IList<AttachmentViewData> Attachments { get; set; }
        public ShortPersonViewData Owner { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime PostedDate { get; set; }
        public string Text { get; set; }
        public bool Hidden { get; set; }
        public bool Deleted { get; set; }
        public IList<AnnouncementCommentViewData> SubComments { get; set; } 

        public static AnnouncementCommentViewData Create(AnnouncementComment announcementComment, IList<AttachmentInfo> attachments, int currentPersonId)
        {
            return new AnnouncementCommentViewData
            {
                Id = announcementComment.Id,
                AnnouncementId = announcementComment.AnnouncementRef,
                Attachments = attachments?.Select(x=>AttachmentViewData.Create(x, currentPersonId)).ToList(),
                ParentCommentId = announcementComment.ParentCommentRef,
                Owner = ShortPersonViewData.Create(announcementComment.Person),
                PostedDate = announcementComment.PostedDate,
                Text = announcementComment.Text,
                Hidden = announcementComment.Hidden,
                Deleted = announcementComment.Deleted
            };
        }
    }
}