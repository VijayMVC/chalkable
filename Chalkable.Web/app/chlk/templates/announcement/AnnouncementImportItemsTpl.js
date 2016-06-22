REQUIRE('chlk.templates.announcement.AnnouncementImportTpl');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementImportItemsTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementImportViewData)],
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementImportItems.jade')],
        'AnnouncementImportItemsTpl', EXTENDS(chlk.templates.announcement.AnnouncementImportTpl), [])
});