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
            Boolean, 'ableDownload',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.ShortUserInfo), 'students',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableToReadSSNumber',

            Boolean, function hideSSNumber(){
                return !this.isAbleToReadSSNumber();
            },

            [[String]],
            ArrayOf(chlk.models.common.ActionLinkModel), function buildReportLinksData(pressedAction){
                var controllerName = 'grading';
                var args = [this.getGradingPeriodId(), this.getClassId(), this.getStartDate().toStandardFormat(), this.getEndDate().toStandardFormat()];
                var classNames = ['report-button'];
                return [
                    new chlk.models.common.ActionLinkModel(controllerName, 'gradeBookReport', 'Grade Book', 'gradeBookReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'worksheetReport', 'Worksheet', 'worksheetReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'progressReport', 'Progress', 'progressReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'comprehensiveProgressReport', 'Comprehensive Progress', 'comprehensiveProgressReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'missingAssignmentsReport', 'Missing Assignments', 'missingAssignmentsReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'birthdayReport', 'Birthday Listing', 'birthdayReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'gradeVerificationReport', 'Grade Verification', 'gradeVerificationReport' == pressedAction, args, classNames),
                    new chlk.models.common.ActionLinkModel(controllerName, 'seatingChartReport', 'Seating Chart', 'seatingChartReport' == pressedAction, args, classNames)
                ];
            }
        ])
});