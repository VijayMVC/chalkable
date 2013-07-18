REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.announcement.AnnouncementForm');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.AnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementForm)],
        'AnnouncementFormPage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});