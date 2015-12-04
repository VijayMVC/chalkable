REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.ComprehensiveProgressReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    var attDisplayMethodEnum = chlk.models.reports.AttendanceDisplayMethodEnum;

    /**@class chlk.activities.reports.ComprehensiveProgressReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ComprehensiveProgressReportTpl)],
        'ComprehensiveProgressReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[

            [ria.mvc.DomEventBind('submit', '.comprehensive-progress-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){

                var reasonsNode = node.find('#absence-reasons'),
                    reasonsArray = node.find('.reasons-select').getValue() || [];
                reasonsNode.setValue(reasonsArray.join(','));

                var yearToDate = node.find('#year-to-date-chk').checked();
                var gradingPeriod = node.find('#grading-period-chk').checked();
                var dailyAttendanceDisplayMethodNode = node.find('#daily-attendance-display-method');
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