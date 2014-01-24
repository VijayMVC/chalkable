REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.Class*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/StudentClassItem.jade')],
        [ria.templates.ModelBind(chlk.models.classes.Class)],
        'Class', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.course.Course, 'course',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevel, 'gradeLevel',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'id',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'teacher'
        ])
});