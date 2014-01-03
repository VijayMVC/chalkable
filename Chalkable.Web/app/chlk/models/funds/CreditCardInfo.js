
NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /** @class chlk.models.funds.CreditCardInfo*/
    CLASS('CreditCardInfo', [
        [ria.serialize.SerializeProperty("cardnumber")],

        Number, 'cardNumber',
        Number, 'month',
        Number, 'year',

        [ria.serialize.SerializeProperty("cvcnumber")],
        Number, 'cvcNumber'
    ]);
});