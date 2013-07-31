REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.class.ClassForTopBar');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.CourseId');

NAMESPACE('chlk.templates.class', function () {

    /** @class chlk.templates.class.TopBar*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/TopBar.jade')],
        [ria.templates.ModelBind(chlk.models.class.ClassForTopBar)],
        'TopBar', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.CourseId, 'courseInfoId',
            [ria.templates.ModelPropertyBind],
            Boolean, 'pressed',
            [ria.templates.ModelPropertyBind],
            Boolean, 'disabled',
            [ria.templates.ModelPropertyBind],
            Number, 'index'
        ])
});