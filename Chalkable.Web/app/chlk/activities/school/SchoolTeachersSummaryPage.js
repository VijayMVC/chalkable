REQUIRE('chlk.activities.district.DistrictSummaryPage');
REQUIRE('chlk.templates.school.SchoolTeachersSummaryTpl');
REQUIRE('chlk.templates.school.SchoolTeachersStatisticTpl');
REQUIRE('chlk.templates.school.SchoolTeachersStatisticItemsTpl');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolTeachersSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolTeachersSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.school.SchoolTeachersStatisticTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.school.SchoolTeachersStatisticItemsTpl, chlk.activities.lib.DontShowLoader(), '.grid-content', ria.mvc.PartialUpdateRuleActions.Append)],
        'SchoolTeachersSummaryPage', EXTENDS(chlk.activities.school.SchoolClassesSummaryPage), []);
});