REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.class.ClassForTopBar');

NAMESPACE('chlk.templates.class', function () {

    /** @class chlk.templates.class.TopBar*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/TopBar.jade')],
        [ria.templates.ModelBind(chlk.models.class.ClassForTopBar)],
        'TopBar', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            chlk.models.class.CourseId, 'courseInfoId',
            [ria.templates.ModelPropertyBind],
            Boolean, 'pressed'
        ])
});