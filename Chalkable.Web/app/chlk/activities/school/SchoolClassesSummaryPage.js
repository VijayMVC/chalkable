REQUIRE('chlk.activities.district.DistrictSummaryPage');
REQUIRE('chlk.templates.school.SchoolClassesSummaryTpl');
REQUIRE('chlk.templates.school.SchoolClassesStatisticTpl');

NAMESPACE('chlk.activities.school', function () {
    var filterTimeout;

    /** @class chlk.activities.school.SchoolClassesSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolClassesSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.school.SchoolClassesStatisticTpl, '', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'SchoolClassesSummaryPage', EXTENDS(chlk.activities.district.DistrictSummaryPage), [

            [ria.mvc.DomEventBind('change', '#school-year-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function yearSelect(node, event, selected_){
                node.parent('form').trigger('submit');
            }
        ]);
});