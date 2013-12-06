REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.Popup*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.Popup)],
        'Popup', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ria.dom.Dom, 'target',
            [ria.templates.ModelPropertyBind],
            ria.dom.Dom, 'container'
        ])
});