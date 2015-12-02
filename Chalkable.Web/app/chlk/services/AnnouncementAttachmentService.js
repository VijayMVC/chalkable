REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');



NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementAttachmentService */
    CLASS(
        'AnnouncementAttachmentService', EXTENDS(chlk.services.BaseService), [


            [[chlk.models.id.AnnouncementId, Object, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function uploadAttachment(announcementId, files, announcementType) {
                return this.uploadFiles('AnnouncementAttachment/UploadAnnouncementAttachment', files, chlk.models.attachment.AnnouncementAttachment, {
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId]],
            ria.async.Future, function addAttachment(announcementId, announcementType, attachmentId) {
                return this.post('AnnouncementAttachment/Add', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf(),
                    attachmentId: attachmentId.valueOf()
                });
            },


            [[chlk.models.id.AnnouncementAttachmentId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function startViewSession(announcementAttachmentId, announcementType_) {
                var annType = announcementType_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                return this.get('AnnouncementAttachment/StartViewSession', String, {
                    announcementAttachmentId: announcementAttachmentId.valueOf(),
                    announcementType: annType.valueOf()
                });
            },

            [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function cloneAttachment(attachmentId, announcementId, announcementType_) {
                //todo : remove getting announcementType  from session
                var annType = announcementType_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                return this.get('AnnouncementAttachment/CloneAttachment', chlk.models.announcement.AnnouncementView, {
                    originalAttachmentId: attachmentId.valueOf(),
                    announcementId: announcementId.valueOf(),
                    announcementType: annType.valueOf()
                });
            },


            [[chlk.models.id.AnnouncementAttachmentId, Boolean, Number, Number]],
            String, function getAttachmentUri(announcementAttachmentId, needsDownload, width, height) {
                return this.getUrl('AnnouncementAttachment/DownloadAttachment', {
                    attachmentId: announcementAttachmentId.valueOf(),
                    needsDownload: needsDownload,
                    width: width,
                    height: height
                });
            },

            [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteAttachment(attachmentId, announcementId, announcementType) {
                return this.get('AnnouncementAttachment/DeleteAttachment.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementAttachmentId: attachmentId.valueOf(),
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            }
    ]);
});
