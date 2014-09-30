REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradeBookReportViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.GradeBookReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradeBookReport.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradeBookReportViewData)],
        'GradeBookReportTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.BaseAnnouncementViewData), 'announcements'
        ])
});