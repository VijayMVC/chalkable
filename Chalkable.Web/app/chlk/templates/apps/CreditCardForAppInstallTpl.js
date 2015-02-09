REQUIRE('chlk.models.funds.AddCreditCardModel');
REQUIRE('chlk.templates.funds.CreditCardTpl');

NAMESPACE('chlk.templates.apps', function () {

    "use strict";

    /** @class chlk.templates.apps.CreditCardForAppInstallTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/CreditCardInfoView.jade')],
        [ria.templates.ModelBind(chlk.models.funds.AddCreditCardModel)],
        'CreditCardForAppInstallTpl', EXTENDS(chlk.templates.funds.CreditCardTpl), [

        ]);
});