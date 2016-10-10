REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.reports.SubmitReportCardsViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.ReportCardsSubmitFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/ReportCardsSubmitForm.jade')],
        [ria.templates.ModelBind(chlk.models.reports.SubmitReportCardsViewData)],
        'ReportCardsSubmitFormTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.reports.CustomReportTemplate), 'templates',

            [ria.templates.ModelPropertyBind],
            chlk.models.reports.StudentIdentifierEnum, 'idToPrint',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableDownload',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableToReadSSNumber',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups',

            Boolean, function hideSSNumber(){
                return !this.isAbleToReadSSNumber();
            },

    ]);
});