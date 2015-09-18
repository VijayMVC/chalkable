REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementAttributeListViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AnnouncementAttributesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAttributes.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementAttributeListViewData)],
        'AnnouncementAttributesTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData), 'announcementAttributes',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly'
    ]);
});