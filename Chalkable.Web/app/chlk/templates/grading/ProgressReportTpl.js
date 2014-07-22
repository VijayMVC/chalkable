REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.SubmitProgressReportViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.ProgressReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/ProgressReport.jade')],
        [ria.templates.ModelBind(chlk.models.grading.SubmitProgressReportViewData)],
        'ProgressReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.UserForReport), 'students',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate'
        ])
});