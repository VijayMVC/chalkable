REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.RequestResultViewData');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.TransactionCompleteTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.RequestResultViewData)],
        [ria.templates.TemplateBind('~/assets/jade/common/TransactionComplete.jade')],
        'TransactionCompleteTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'text',

            [ria.templates.ModelPropertyBind],
            Boolean, 'success'
    ]);
});