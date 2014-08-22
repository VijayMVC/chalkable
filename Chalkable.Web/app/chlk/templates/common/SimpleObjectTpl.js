REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.common.SimpleObject');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.SimpleObjectTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.SimpleObject)],
        [ria.templates.TemplateBind('~/assets/jade/controls/paginator.jade')],
        'SimpleObjectTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Object, 'value'
        ])
});