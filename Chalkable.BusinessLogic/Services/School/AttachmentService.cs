using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAttachementService
    {
        Attachment Upload(string name, byte[] content, bool uploadToSti = false);
        void Delete(int attachmentId);
        Attachment GetAttachmentById(int attachmentId);
        AttachmentContentInfo GetAttachmentContent(int attachmentId);
        AttachmentContentInfo GetAttachmentContent(Attachment attachment);
        PaginatedList<AttachmentInfo> GetAttachmentsInfo(string filter, AttachmentSortTypeEnum? sortType, int start = 0, int count = Int32.MaxValue);
        AttachmentInfo TransformToAttachmentInfo(Attachment attachment, IList<int> teacherIds = null);
    }


    public enum AttachmentSortTypeEnum
    {
        RecentlySent = 1,
        NewestUploaded,
        OldestUploaded
    }

    public class AttachmentService : SisConnectedService, IAttachementService
    {
        public AttachmentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Attachment Upload(string name, byte[] content, bool uploadToSti = false)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Attachment res = null;
            DoUpdate(u => { res = Upload(name, content, uploadToSti, u);});
            return res;
        }

        public Attachment Upload(string name, byte[] content, bool uploadToSti, UnitOfWork unitOfWork)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var res = new Attachment
                {
                    Name = name,
                    PersonRef = Context.PersonId.Value,
                    Uuid = UploadToCrocodoc(name, content),
                    UploadedDate = Context.NowSchoolTime,
                    LastAttachedDate = Context.NowSchoolTime
                };
            var da = new AttachmentDataAccess(unitOfWork);
            if (uploadToSti)
            {
                var stiAtt = ConnectorLocator.AttachmentConnector.UploadAttachment(name, content).Last();

                //TODO : use mapping 
                res.Name = stiAtt.Name;
                res.SisAttachmentId = stiAtt.AttachmentId;
                res.MimeType = stiAtt.MimeType;
                da.Insert(res);
                return da.GetLast(Context.PersonId.Value);
            }
            da.Insert(res);
            res = da.GetLast(Context.PersonId.Value);
            res.RelativeBlobAddress = UploadToBlob(res, content);
            da.Update(res);
            return res;
        }

        public void Delete(int attachmentId)
        {
            DoUpdate(u =>
                {
                    var da = new AttachmentDataAccess(u);
                    var att = da.GetById(attachmentId);
                    if(att.PersonRef != Context.PersonId)
                        throw new ChalkableSecurityException();
                    da.Delete(attachmentId);
                    if(att.SisAttachmentId.HasValue)
                        ConnectorLocator.AttachmentConnector.DeleteAttachment(att.SisAttachmentId.Value);
                    else 
                        ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, att.RelativeBlobAddress);
                });
        }

        public AttachmentContentInfo GetAttachmentContent(int attachmentId)
        {
            return GetAttachmentContent(GetAttachmentById(attachmentId));
        }

        public AttachmentContentInfo GetAttachmentContent(Attachment attachment)
        {
            var res = new AttachmentContentInfo { Attachment = attachment };
            if (attachment.SisAttachmentId.HasValue)
                res.Content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(attachment.SisAttachmentId.Value);
            else
                res.Content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, attachment.RelativeBlobAddress);
            return res;
        }

        public Attachment GetAttachmentById(int attachmentId)
        {
            return DoRead(u => new AttachmentDataAccess(u).GetById(attachmentId));
        }

        public PaginatedList<AttachmentInfo> GetAttachmentsInfo(string filter, AttachmentSortTypeEnum? sortType, int start = 0, int count = Int32.MaxValue)
        {
            Trace.Assert(Context.PersonId.HasValue);
            string orderByColumn = null; 
            Orm.OrderType orderType = Orm.OrderType.Desc;
            sortType = sortType ?? AttachmentSortTypeEnum.RecentlySent;
            switch (sortType)
            {
                case AttachmentSortTypeEnum.RecentlySent:
                    orderByColumn = Attachment.LAST_ATTACHED_DATE_FIELD;
                    break;
                case AttachmentSortTypeEnum.NewestUploaded:
                    orderByColumn = Attachment.UPLOADED_DATE_FIELD;
                    break;
                case AttachmentSortTypeEnum.OldestUploaded:
                    orderByColumn = Attachment.UPLOADED_DATE_FIELD;
                    orderType = Orm.OrderType.Asc;
                break;
            }
            var res = DoRead(u => new AttachmentDataAccess(u).GetPaginatedAttachments(Context.PersonId.Value, filter, orderByColumn, orderType, start, count));
            var teacherIds = new List<int> {Context.PersonId.Value};
            return res.Transform(x => TransformToAttachmentInfo(x, teacherIds));
        }

        public AttachmentInfo TransformToAttachmentInfo(Attachment attachment, IList<int> teacherIds = null)
        {
            var docWidth = Attachment.DOCUMENT_DEFAULT_WIDTH;
            var docHeight = Attachment.DOCUMENT_DEFAULT_HEIGHT;
            return new AttachmentInfo
                {
                    Attachment = attachment,
                    DocWidth = docWidth,
                    DocHeight = docHeight,
                    IsTeacherAttachment = teacherIds != null && teacherIds.Contains(attachment.PersonRef),
                    DownloadDocumentUrl = ServiceLocator.CrocodocService.BuildDownloadDocumentUrl(attachment.Uuid, attachment.Name),
                    DownloadThumbnailUrl = ServiceLocator.CrocodocService.BuildDownloadhumbnailUrl(attachment.Uuid, docWidth, docHeight)
                };
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";
        
        private string UploadToBlob(Attachment attachment, byte[] content)
        {
            return UploadToBlob(new List<AttachmentContentInfo>
                {
                    new AttachmentContentInfo{Attachment = attachment, Content = content}
                }).First();
        }
        private IList<string> UploadToBlob(IList<AttachmentContentInfo> attachmentContent)
        {
            var res = new List<string>();
            foreach (var attachmentContentInfo in attachmentContent)
            {
                var key = GenerateKeyForBlob(attachmentContentInfo.Attachment);
                ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, key, attachmentContentInfo.Content);
                res.Add(key);
            }
            return res;
        }

        private string GenerateKeyForBlob(Attachment attachment)
        {
            var res = attachment.Id.ToString(CultureInfo.InvariantCulture);
            return Context.DistrictId.HasValue ? string.Format("{0}_{1}", res, Context.DistrictId.Value) : res;
        }

        private string UploadToCrocodoc(string name, byte[] content)
        {
            if (ServiceLocator.CrocodocService.IsDocument(name))
                return ServiceLocator.CrocodocService.UploadDocument(name, content).uuid;
            return null;
        }

    }


    public class AttachmentInfo
    {
        public Attachment Attachment { get; set; }
        public int DocWidth { get; set; }
        public int DocHeight { get; set; }
        public string DownloadDocumentUrl { get; set; }
        public string DownloadThumbnailUrl { get; set; }
        public bool IsTeacherAttachment { get; set; }

    }

}
