REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.MissingAssignmentsReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.MissingAssignmentsReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.MissingAssignmentsReportTpl)],
        'MissingAssignmentsReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[

            [ria.mvc.DomEventBind('submit', '.missing-assignments-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var alternateScoresNode = node.find('#alternate-scores'),
                    alternateScoresIds = node.find('.alternate-scores-select').getValue() || [];
                alternateScoresNode.setValue(alternateScoresIds.join(','));
            }
        ]);
});