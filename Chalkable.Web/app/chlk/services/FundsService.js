REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.funds.Fund');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.FundsService */
    CLASS(
        'FundsService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getFunds(pageIndex_) {
                return this.getPaginatedList('chalkable2/app/data/funds.json', chlk.models.funds.Fund, {
                    start: pageIndex_
                });
            },

            ria.async.Future, function getBalance() {
                return this.get('Fund/GetAppBudgetBalance.json', chlk.models.funds.BudgetBalance, {});
            }
        ])
});