REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /**@class chlk.models.funds.FundsHistory*/

    CLASS('FundsHistory',[

        String, 'title',
        Number, 'amount',
        Number, 'balance',
        chlk.models.common.ChlkDate, 'created'
    ]);
});