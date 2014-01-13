REQUIRE('chlk.models.funds.CreditCardInfo');

NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /** @class chlk.models.funds.AddCreditCardModel*/
    CLASS('AddCreditCardModel', EXTENDS(chlk.models.funds.CreditCardInfo),[
        Number, 'amount'
    ]);
});