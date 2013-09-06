REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.classes.ClassForTopBar');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.CourseId');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.TopBar*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/TopBar.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassForTopBar)],
        'TopBar', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            chlk.models.course.Course, 'course',
            [ria.templates.ModelPropertyBind],
            Boolean, 'pressed',
            [ria.templates.ModelPropertyBind],
            Boolean, 'disabled',
            [ria.templates.ModelPropertyBind],
            Number, 'index',
            [ria.templates.ModelPropertyBind],
            String, 'controller',
            [ria.templates.ModelPropertyBind],
            String, 'action',
            [ria.templates.ModelPropertyBind],
            Array, 'params'
        ])
});