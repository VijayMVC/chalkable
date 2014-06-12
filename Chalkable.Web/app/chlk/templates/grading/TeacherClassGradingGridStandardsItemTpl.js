REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.StandardGrading');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridStandardsItem.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardGrading)],
        'TeacherClassGradingGridStandardsItemTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradeId, 'gradeId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            String, 'gradeValue',

            [ria.templates.ModelPropertyBind],
            String, 'comment'
        ])
});