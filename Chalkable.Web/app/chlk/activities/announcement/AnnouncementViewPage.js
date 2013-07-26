REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementForm');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.class.TopBar');

NAMESPACE('chlk.activities.announcement', function () {

    var handler;

    /** @class chlk.activities.announcement.AnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],

        'AnnouncementFormView', EXTENDS(chlk.activities.lib.TemplatePage), []
    );
});