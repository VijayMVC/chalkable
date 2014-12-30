REQUIRE('chlk.models.funds.Fund');

NAMESPACE('chlk.models.funds', function () {
    "use strict";

    /** @class chlk.models.funds*/
    CLASS(
        'FundsList', [
            ArrayOf(chlk.models.funds.Fund), 'items'
        ]);
});