REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.funds.Fund');
REQUIRE('chlk.models.funds.SchoolPersonFundsViewData');

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
            },
            ria.async.Future, function getPersonFunds(){
                return this.get('Fund/PersonFunds.json', chlk.models.funds.SchoolPersonFundsViewData,{});
            },
            [[Number, String, Number, Number, Number, String]],
            ria.async.Future, function addCredit(amount, cardNumber, month, year, cvcNumber, cardType){
                return this.post("Fund/AddCredit.json", Boolean, {
                    amount: amount,
                    cardNumber: cardNumber,
                    month: month,
                    year: year,
                    cvcNumber: cvcNumber,
                    cardType: cardType
                });
            }
        ])
});