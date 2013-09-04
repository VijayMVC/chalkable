REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.Summary');
REQUIRE('chlk.templates.classes.Class');
REQUIRE('chlk.templates.announcement.AnnouncementClassPeriod');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.Summary)],
        'SummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});