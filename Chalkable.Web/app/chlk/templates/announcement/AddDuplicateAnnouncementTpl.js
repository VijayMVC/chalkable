REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AddDuplicateAnnouncementViewData');
REQUIRE('chlk.templates.classes.TopBar');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AddDuplicateAnnouncementTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AddDuplicateAnnouncement.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AddDuplicateAnnouncementViewData)],
        'AddDuplicateAnnouncementTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.classes.ClassForTopBar), 'classes', //todo: rename
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'selectedClassId',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',
            [ria.templates.ModelPropertyBind],
            String, 'selectedIds'
        ]);
});