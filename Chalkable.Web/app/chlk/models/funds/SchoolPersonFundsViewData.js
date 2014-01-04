REQUIRE('chlk.models.funds.FundsHistory');
REQUIRE('chlk.models.funds.CreditCardInfo');

NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /**@class chlk.models.funds.SchoolPersonFundsViewData*/

    CLASS('SchoolPersonFundsViewData', [
        [ria.serialize.SerializeProperty('fundshistory')],
        ArrayOf(chlk.models.funds.FundsHistory), 'fundsHistory',

        [ria.serialize.SerializeProperty('creditcardinfo')],
        chlk.models.funds.CreditCardInfo, 'creditCardInfo',

        [ria.serialize.SerializeProperty('currentbalance')],
        Number, 'currentBalance'
    ]);
});