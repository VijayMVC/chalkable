REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfileSummaryTpl');
REQUIRE('chlk.templates.classes.Class');
REQUIRE('chlk.templates.announcement.AnnouncementClassPeriod');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileSummaryTpl)],
        'StudentProfileSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});