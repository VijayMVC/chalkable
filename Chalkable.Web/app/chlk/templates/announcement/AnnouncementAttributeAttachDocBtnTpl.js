REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AnnouncementAttributeAttachDocBtnTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAttributeAttachDocBtn.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementAttributeViewData)],
        'AnnouncementAttributeAttachDocBtnTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAssignedAttributeId, 'id',

            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementAttributeAttachmentViewData, 'attributeAttachment'


    ]);
});