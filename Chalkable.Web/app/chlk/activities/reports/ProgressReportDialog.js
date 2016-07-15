REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.ProgressReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.ProgressReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ProgressReportTpl)],
        'ProgressReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[

            [ria.mvc.DomEventBind('change', '.category-average')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function categoryAverageChange(node, event){
                node.parent('.item-3').find('.small-input').setAttr('disabled', !node.checked());
            },

            [ria.mvc.DomEventBind('change', '.reasons-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function reasonsSelectChange(node, event, options_){
                _DEBUG && console.info(node.getValue(), options_);
            },

            [ria.mvc.DomEventBind('submit', '.progress-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var commentsNode = node.find('#coments-list'),
                    notSelectedNode = node.find('#not-selected-count'),
                    commentsArray = [], comment, notSelectedCount = 0, id;
                node.find('.student-chk').forEach(function(item){
                    comment = item.parent('.student-item').find('.student-comment').getValue();
                    id = item.parent('.student-item').find('.student-chk').getData('id');
                    if(item.is(':checked'))
                        commentsArray.push({
                            studentId:id,
                            comment: comment
                        });
                    else
                        if(comment)
                            notSelectedCount++;

                });

                notSelectedNode.setValue(notSelectedCount);

                if(commentsArray.length)
                    commentsNode.setValue(JSON.stringify(commentsArray));

                var yearToDate = node.find('#year-to-date-chk').checked();
                var gradingPeriod = node.find('#grading-period-chk').checked();
                var dailyAttendanceDisplayMethodNode = node.find('#daily-attendance-display-method');
                var reasonsNode = node.find('#absence-reasons'),
                    reasonsArray = node.find('.reasons-select').getValue();
                if(reasonsArray && reasonsArray.length)
                    reasonsNode.setValue(reasonsArray.join(','));

                dailyAttendanceDisplayMethodNode.setValue(this.getAttDisplayMethod(yearToDate, gradingPeriod).valueOf());
            },

            [[Boolean, Boolean]],
            chlk.models.reports.AttendanceDisplayMethodEnum, function getAttDisplayMethod(isYearToDate, isGradingPeriodNode){
                if(isYearToDate && isGradingPeriodNode)
                    return chlk.models.reports.AttendanceDisplayMethodEnum.BOTH;
                if(isYearToDate)
                    return chlk.models.reports.AttendanceDisplayMethodEnum.YEAR_TO_DATE;
                if(isGradingPeriodNode)
                    return chlk.models.reports.AttendanceDisplayMethodEnum.GRADING_PERIOD;
                return chlk.models.reports.AttendanceDisplayMethodEnum.NONE;
            }
        ]);
});