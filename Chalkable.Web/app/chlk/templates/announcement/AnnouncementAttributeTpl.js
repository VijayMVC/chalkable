REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AnnouncementAttributeTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAttribute.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementAttributeViewData)],
        'AnnouncementAttributeTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAssignedAttributeId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'text',

            [ria.templates.ModelPropertyBind],
            String, 'uuid',

            [ria.templates.ModelPropertyBind],
            Boolean, 'visibleForStudents',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAttributeTypeId, 'attributeTypeId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementRef',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementAttributeAttachmentViewData, 'attributeAttachment',

            [ria.templates.ModelPropertyBind],
            Boolean, 'deleted',

            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly',

            [ria.templates.ModelPropertyBind],
            Number, 'index',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementAttributeType), 'attributeTypes',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SisAssignedAttributeId, 'sisActivityAssignedAttributeId'

    ]);
});