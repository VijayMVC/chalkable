REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.School*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.Popup)],
        'Popup', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ria.dom.Dom, 'target',
            [ria.templates.ModelPropertyBind],
            ria.dom.Dom, 'container'
        ])
});