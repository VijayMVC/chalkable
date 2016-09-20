REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.Success');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.SuccessTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.Success)],
        [ria.templates.TemplateBind('~/assets/jade/controls/paginator.jade')],
        'SuccessTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'data'
        ])
});