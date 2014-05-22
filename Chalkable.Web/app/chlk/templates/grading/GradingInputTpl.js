REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.GradingInputTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingInput.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ShortStudentAnnouncementViewData)],
        'GradingInputTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            Boolean, 'ableDropStudentScore',

            Boolean, 'ableExemptStudentScore',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            String, 'comment',

            [ria.templates.ModelPropertyBind],
            Boolean, 'dropped',

            [ria.templates.ModelPropertyBind],
            String, 'gradeValue',

            [ria.templates.ModelPropertyBind],
            Number, 'numericGradeValue',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StudentAnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            Number, 'state',

            [ria.templates.ModelPropertyBind],
            Boolean, 'late',

            [ria.templates.ModelPropertyBind],
            Boolean, 'absent',

            [ria.templates.ModelPropertyBind],
            Boolean, 'incomplete',

            [ria.templates.ModelPropertyBind],
            Boolean, 'exempt',

            [ria.templates.ModelPropertyBind],
            Boolean, 'passed',

            [ria.templates.ModelPropertyBind],
            Boolean, 'complete'
        ])
});