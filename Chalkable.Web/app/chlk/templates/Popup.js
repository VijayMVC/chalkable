REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.School*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.Popup)],
        'Popup', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            ria.dom.Dom, 'target',
            [ria.templates.ModelBind],
            ria.dom.Dom, 'container'
        ])
});