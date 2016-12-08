REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfilePanoramaTpl');
REQUIRE('chlk.templates.student.StudentProfilePanoramaStatsTpl');
REQUIRE('chlk.templates.student.StudentProfilePanoramaDisciplineStatsTpl');
REQUIRE('chlk.templates.student.StudentProfilePanoramaAttendanceStatsTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfilePanoramaPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfilePanoramaTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaStatsTpl, '', '.panorama-stats', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaAttendanceStatsTpl, 'sort-attendances', '.attendance-grid', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaDisciplineStatsTpl, 'sort-disciplines', '.discipline-grid', ria.mvc.PartialUpdateRuleActions.Replace)],
        'StudentProfilePanoramaPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.PartialUpdateRule(null, 'save-filters')],
            VOID, function afterFiltersSave(tpl, model, msg_) {

            }
        ]);
});