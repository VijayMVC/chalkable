REQUIRE('chlk.activities.district.DistrictSummaryPage');
REQUIRE('chlk.templates.school.ClassesForStudentTpl');

NAMESPACE('chlk.activities.school', function () {
    var filterTimeout;

    /** @class chlk.activities.school.StudentClassesSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.ClassesForStudentTpl)],
        'StudentClassesSummaryPage', EXTENDS(chlk.activities.district.DistrictSummaryPage), [

            [ria.mvc.DomEventBind('change', '.grading-period-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function yearSelect(node, event, selected_){
                this.dom.find('.gp-submit').trigger('click');
            }
        ]);
});