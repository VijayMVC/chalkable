REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.GradeVerificationReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.GradeVerificationReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.GradeVerificationReportTpl)],
        'GradeVerificationReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[

            [ria.mvc.DomEventBind('submit', '.grade-verification-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var gpNode = node.find('#grading-period-ids'),
                    saNode = node.find('#student-averages-ids'),
                    gpArray = node.find('#grading-period-select').getValue() || [],
                    saArray = node.find('#student-averages-select').getValue() || [];
                gpNode.setValue(gpArray.join(','));
                saNode.setValue(saArray.join(','));
            }
        ]);
});