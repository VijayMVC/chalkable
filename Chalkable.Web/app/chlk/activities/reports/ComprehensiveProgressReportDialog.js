REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.ComprehensiveProgressReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    var attDisplayMethodEnum = chlk.models.reports.AttendanceDisplayMethodEnum;

    /**@class chlk.activities.reports.ComprehensiveProgressReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ComprehensiveProgressReportTpl)],
        'ComprehensiveProgressReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('submit', '.comprehensive-progress-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){

                var yearToDate = node.find('#year-to-date-chk').checked();
                var gradingPeriod = node.find('#grading-period-chk').checked();
                var dailyAttendanceDisplayMethodNode = node.find('#daily-attendance-display-method');
                var reasonsNode = node.find('#absence-reasons'),
                    reasonsArray = node.find('.reasons-select').getValue();
                if(reasonsArray && reasonsArray.length)
                    reasonsNode.setValue(reasonsArray.join(','));

                var gradingPeriodsIdsNode = node.find('#grading-periods'),
                    gradingPeriodsIds = node.find('.grading-periods-select').getValue();
                if(gradingPeriodsIds && gradingPeriodsIds.length > 0)
                    gradingPeriodsIdsNode.setValue(gradingPeriodsIds.join(','));

                dailyAttendanceDisplayMethodNode.setValue(this.getAttDisplayMethod(yearToDate, gradingPeriod).valueOf());
            },

            [[Boolean, Boolean]],
            attDisplayMethodEnum, function getAttDisplayMethod(isYearToDate, isGradingPeriodNode){
                if(isYearToDate && isGradingPeriodNode)
                    return attDisplayMethodEnum.BOTH;
                if(isYearToDate)
                    return attDisplayMethodEnum.YEAR_TO_DATE;
                if(isGradingPeriodNode)
                    return attDisplayMethodEnum.GRADING_PERIOD;
                return attDisplayMethodEnum.NONE;
            }
    ]);
});