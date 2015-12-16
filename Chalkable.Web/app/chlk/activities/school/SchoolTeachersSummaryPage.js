REQUIRE('chlk.activities.district.DistrictSummaryPage');
REQUIRE('chlk.templates.school.SchoolTeachersSummaryTpl');
REQUIRE('chlk.templates.school.SchoolTeachersStatisticTpl');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolTeachersSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolTeachersSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.school.SchoolTeachersStatisticTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'SchoolTeachersSummaryPage', EXTENDS(chlk.activities.school.SchoolClassesSummaryPage), []);
});