using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementComment
    {
        public const string VW_ANNOUNCEMENT_COMMENT = "vwAnnouncementComment";

        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        //public int? AttachmentRef { get; set; }
        public int PersonRef { get; set; }
        public int? ParentCommentRef { get; set; }
        public string Text { get; set; }
        public DateTime PostedDate { get; set; }
        public bool Hidden { get; set; }
        public bool Deleted { get; set; }
        
        [NotDbFieldAttr]
        public IList<Attachment> Attachments { get; set; }
        [DataEntityAttr]
        public Person Person { get; set; }

        [NotDbFieldAttr]
        public IList<AnnouncementComment> SubComments { get; set; }

        public IList<AnnouncementComment> AllSubComments
        {
            get
            {
                var res = new List<AnnouncementComment>();
                if (SubComments != null)
                {
                    foreach (var comment in SubComments)
                    {
                        res.Add(comment);
                        res.AddRange(comment.AllSubComments);
                    }
                }
                return res;
            }    
        } 

    }


    public class AnnouncementCommentAttachment
    {
        [PrimaryKeyFieldAttr]
        public int AnnouncementCommentRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int AttachmentRef { get; set; }

        [DataEntityAttr]
        public Attachment Attachment { get; set; }
    }
}
