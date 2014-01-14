REQUIRE('chlk.models.funds.CreditCardInfo');

NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /** @class chlk.models.funds.AddCreditCardModel*/
    CLASS('AddCreditCardModel', [

        chlk.models.funds.CreditCardInfo, 'creditCardInfo',
        Number, 'amount',

        [[chlk.models.funds.CreditCardInfo, Number]],
        function $(creditCardInfo_, amount_){
            BASE();
            if(creditCardInfo_)
                this.setCreditCardInfo(creditCardInfo_);
            if(amount_)
                this.setAmount(amount_);
        }

    ]);
});