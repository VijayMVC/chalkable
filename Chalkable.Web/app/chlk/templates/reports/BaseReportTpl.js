REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.reports.BaseReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    ASSET('~/assets/jade/activities/reports/ReportBase.jade')();

    /** @class chlk.templates.reports.BaseReportTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.reports.BaseReportViewData)],
        'BaseReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',


            [[String]],
            ArrayOf(chlk.models.common.ActionLinkModel), function buildReportLinksData(pressedAction){
                var controllerName = 'grading';
                var args = [this.getGradingPeriodId(), this.getClassId(), this.getStartDate().toStandardFormat(), this.getEndDate().toStandardFormat()];
                var classNames = ['report-button'];
                return [
                    new chlk.models.common.ActionLinkModel(controllerName, 'gradeBookReport', Msg.Grade_Book_Report, 'gradeBookReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'worksheetReport', Msg.Worksheet_Report, 'worksheetReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'progressReport', Msg.Progress_Report, 'progressReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'comprehensiveProgressReport', Msg.Comprehensive_Progress_Report, 'comprehensiveProgressReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'missingAssignmentsReport', Msg.Missing_Assignments_Report, 'missingAssignmentsReport' == pressedAction, args, classNames)
                ];
            }
        ])
});