REQUIRE('chlk.models.funds.CreditCardInfo');

NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /** @class chlk.models.funds.AddCreditCardModel*/
    CLASS('AddCreditCardModel', EXTENDS(chlk.models.funds.CreditCardInfo), [

        Number, 'amount',

        [[chlk.models.funds.CreditCardInfo, Number]],
        function $create(creditCardInfo_, amount_){
            BASE();
            if(creditCardInfo_)
                this.setCreditCardInfo_(creditCardInfo_);
            if(amount_)
                this.setAmount(amount_);
        },

        [[chlk.models.funds.CreditCardInfo]],
        VOID, function setCreditCardInfo_(creditCardData){
            this.initData_(
                creditCardData.getCardNumber(),
                creditCardData.getMonth(),
                creditCardData.getYear(),
                creditCardData.getCvcNumber(),
                creditCardData.getCardType()
            );
        }
    ]);
});