REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.class.Class');

NAMESPACE('chlk.templates.class', function () {

    /** @class chlk.templates.class.Class*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/StudentClassItem.jade')],
        [ria.templates.ModelBind(chlk.models.class.Class)],
        'Class', EXTENDS(chlk.templates.JadeTemplate), [
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