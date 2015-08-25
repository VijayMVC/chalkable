REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AnnouncementAttributeAttachmentTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAttributeAttachment.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementAttributeAttachmentViewData)],
        'AnnouncementAttributeAttachmentTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAssignedAttributeAttachmentId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'url',

            [ria.templates.ModelPropertyBind],
            String, 'thumbnailUrl',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.AttachmentTypeEnum, 'type',

            [ria.templates.ModelPropertyBind],
            String, 'uuid',

            [ria.templates.ModelPropertyBind],
            String, 'mimeType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'stiAttachment',

            //

            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAssignedAttributeId, 'attributeId',

    ]);
});