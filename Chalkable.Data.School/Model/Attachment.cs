using System;
using Chalkable.Common.Web;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Attachment
    {
        public const int DOCUMENT_DEFAULT_WIDTH = 110;
        public const int DOCUMENT_DEFAULT_HEIGHT = 170;

        public const string ID_FIELD = "Id";
        public const string NAME_FIELD = "Name";
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string UPLOADED_DATE_FIELD = "UploadedDate";
        public const string LAST_ATTACHED_DATE_FIELD = "LastAttachedDate";
        public const string SIS_ATTACHMENT_ID_FIELD = "SisAttachmentId";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int PersonRef { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
	    public DateTime UploadedDate { get; set; } 
	    public DateTime LastAttachedDate { get; set; }
	    public string Uuid { get; set; }
	    public int? SisAttachmentId { get; set; } 
        public string RelativeBlobAddress { get; set; }

        [NotDbFieldAttr]
        public bool IsDocument => MimeHelper.GetTypeByName(Name) == MimeHelper.AttachmenType.Document;

        [NotDbFieldAttr]
        public bool IsStiAttachment => SisAttachmentId.HasValue;
    }
}
