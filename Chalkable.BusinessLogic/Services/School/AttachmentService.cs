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
using Chalkable.StiConnector.Connectors;

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
        IList<Attachment> UploadToCrocodoc(IList<Attachment> attachments);

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
            DoUpdate(u => { res = Upload(name, content, uploadToSti, u, ServiceLocator, ConnectorLocator);});
            return res;
        }

        public static Attachment Upload(string name, byte[] content, bool uploadToSti, UnitOfWork unitOfWork, IServiceLocatorSchool serviceLocator, 
            ConnectorLocator connectorLocator, bool uploadToCrocodoc = true)
        {
            var context = serviceLocator.Context;
            Trace.Assert(context.PersonId.HasValue);
            var res = new Attachment
                {
                    Name = name,
                    PersonRef = context.PersonId.Value,
                    Uuid = null,
                    UploadedDate = context.NowSchoolTime,
                    LastAttachedDate = context.NowSchoolTime
                };

            if(uploadToCrocodoc)
                 res.Uuid = UploadToCrocodoc(res.Name, content, serviceLocator);
           
            var da = new AttachmentDataAccess(unitOfWork);
            if (uploadToSti)
            {
                var stiAtt = connectorLocator.AttachmentConnector.UploadAttachment(name, content).Last();

                //TODO : use mapping 
                res.Name = stiAtt.Name;
                res.SisAttachmentId = stiAtt.AttachmentId;
                res.MimeType = stiAtt.MimeType;
                da.Insert(res);
                return da.GetLast(context.PersonId.Value);
            }
            da.Insert(res);
            res = da.GetLast(context.PersonId.Value);
            res.RelativeBlobAddress = UploadToBlob(res, content, serviceLocator);
            da.Update(res);
            return res;
        }


        private const int UPLOAD_PACKET_SIZE = 3;
        public IList<Attachment> UploadToCrocodoc(IList<Attachment> attachments)
        {
            if (attachments == null || attachments.Count == 0)
                return new List<Attachment>();

            var filtered = attachments.Where(x => ServiceLocator.CrocodocService.IsDocument(x.Name)
                && string.IsNullOrWhiteSpace(x.Uuid)).ToList();
            
            for (var i = 0; i < filtered.Count; i += UPLOAD_PACKET_SIZE)
            {
                var attsForUpload = filtered.Skip(i).Take(UPLOAD_PACKET_SIZE).ToList();
                foreach (var item in attsForUpload)
                {
                    var attcontent = GetAttachmentContent(item);
                    item.Uuid = UploadToCrocodoc(item.Name, attcontent.Content, ServiceLocator);
                }


                ServiceLocator.CrocodocService.WaitForDocuments(attsForUpload.Select(x => x.Uuid).ToList());
            }
            DoUpdate(u=> new AttachmentDataAccess(u).Update(filtered));
            return filtered;
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
        
        private static string UploadToBlob(Attachment attachment, byte[] content, IServiceLocatorSchool serviceLocator)
        {
            return UploadToBlob(new List<AttachmentContentInfo>
                {
                    new AttachmentContentInfo{Attachment = attachment, Content = content}
                }, serviceLocator).First();
        }
        private static IList<string> UploadToBlob(IList<AttachmentContentInfo> attachmentContent, IServiceLocatorSchool serviceLocator)
        {
            var res = new List<string>();
            foreach (var attachmentContentInfo in attachmentContent)
            {
                var key = GenerateKeyForBlob(attachmentContentInfo.Attachment, serviceLocator.Context);
                serviceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, key, attachmentContentInfo.Content);
                res.Add(key);
            }
            return res;
        }

        private static string GenerateKeyForBlob(Attachment attachment, UserContext context)
        {
            var res = attachment.Id.ToString(CultureInfo.InvariantCulture);
            return context.DistrictId.HasValue ? $"{res}_{context.DistrictId.Value}" : res;
        }
        
        private static string UploadToCrocodoc(string name, byte[] content, IServiceLocator serviceLocator)
        {
            return serviceLocator.CrocodocService.IsDocument(name) 
                ? serviceLocator.CrocodocService.UploadDocument(name, content).uuid : null;
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
