REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.class.TopBar');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.AnnouncementViewPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],
        'AnnouncementViewPage', EXTENDS(chlk.activities.lib.TemplatePage), []
    );
});