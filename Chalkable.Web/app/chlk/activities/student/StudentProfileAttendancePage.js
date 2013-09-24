REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfileAttendanceTpl');
//REQUIRE('chlk.templates.classes.Class');
//REQUIRE('chlk.templates.announcement.AnnouncementClassPeriod');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileAttendancePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileAttendanceTpl)],
        'StudentProfileAttendancePage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});