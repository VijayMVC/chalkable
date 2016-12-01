REQUIRE('chlk.templates.classes.Class');
REQUIRE('chlk.models.classes.Class');


NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassPopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassPopup.jade')],
        [ria.templates.ModelBind(chlk.models.classes.Class)],
        'ClassPopupTpl', EXTENDS(chlk.templates.classes.Class), [

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Room, 'room',

            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'periods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'dayTypes',
        ]);
});