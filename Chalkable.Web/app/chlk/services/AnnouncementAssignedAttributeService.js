REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeAttachmentId');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');
REQUIRE('chlk.models.announcement.AnnouncementAttributeType');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementAssignedAttributeService */
    CLASS(
        'AnnouncementAssignedAttributeService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, Object]],
            ria.async.Future, function uploadAttributeAttachment(announcementType, announcementId, assignedAttributeId, files) {
                return this.uploadFiles('AnnouncementAttribute/UploadAttachment.json', files, chlk.models.announcement.AnnouncementAttributeViewData, {
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf(),
                    assignedAttributeId: assignedAttributeId.valueOf()
                });
            },

            [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.id.AttachmentId]],
            ria.async.Future, function addAttachment(announcementType, announcementId, assignedAttributeId, attachmentId){
                return this.post('AnnouncementAttribute/AddAttachment.json', chlk.models.announcement.AnnouncementAttributeViewData,{
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf(),
                    assignedAttributeId: assignedAttributeId.valueOf(),
                    attachmentId: attachmentId.valueOf()
                });
            },


            [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId]],
            ria.async.Future, function cloneAttachmentForAttribute(attachmentId, announcementId, announcementType, assignedAttributeId) {
                return this.post('AnnouncementAttribute/CloneAttachment', chlk.models.announcement.AnnouncementAttributeViewData, {
                    attachmentId: attachmentId.valueOf(),
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf(),
                    announcementAssignedAttributeId: assignedAttributeId.valueOf()
                });
            },


            [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId]],
            ria.async.Future, function removeAttributeAttachment(announcementType, announcementId, attributeId) {
                return this.post('AnnouncementAttribute/RemoveAttachment.json', chlk.models.announcement.AnnouncementAttributeViewData, {
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf(),
                    announcementAssignedAttributeId: attributeId.valueOf()
                });
            },


            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAttributeTypeId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function addAnnouncementAttribute(announcementId, attributeTypeId, announcementType){
                return this.post('AnnouncementAttribute/AddAttribute.json', chlk.models.announcement.AnnouncementAttributeViewData, {
                    announcementId: announcementId.valueOf(),
                    attributeTypeId: attributeTypeId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function removeAnnouncementAttribute(announcementId, attributeId, announcementType){
                return this.post('AnnouncementAttribute/DeleteAttribute.json', Boolean, {
                    announcementId: announcementId.valueOf(),
                    announcementAssignedAttributeId: attributeId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementAssignedAttributeId]],
            ria.async.Future, function startAttributeAttachmentViewSession(assignedAttributeId) {
                return this.get('AnnouncementAttribute/StartViewSession', String, {
                    assignedAttributeId: assignedAttributeId.valueOf(),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            },


            [[chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum, Boolean, Number, Number]],
            String, function getAttributeAttachmentUri(assignedAttributeId, announcementType, needsDownload, width, height) {
                return this.getUrl('AnnouncementAttribute/DownloadAttributeAttachment', {
                    assignedAttributeId: assignedAttributeId.valueOf(),
                    announcementType:announcementType.valueOf(),
                    needsDownload: needsDownload,
                    width: width,
                    height: height
                });
            },


            [[String]],
            ria.async.Future, function getAnnouncementAttributeTypesSync(query_) {

                var attrs = this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_ATTRIBUTES, []);

                if (query_){
                    query_ = query_.toLowerCase();
                    attrs = attrs.filter(function(item){
                        return item != null && item.getName().toLowerCase().indexOf(query_) != -1;
                    });
                }
                return new ria.async.DeferredData(attrs);
            },

            ArrayOf(chlk.models.announcement.AnnouncementAttributeType), function getAnnouncementAttributeTypesList() {
                return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_ATTRIBUTES, []);
            }
        ])
});