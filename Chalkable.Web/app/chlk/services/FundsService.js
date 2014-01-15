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
                })
                .then(function(data){
                    var cardData = new chlk.models.funds.CreditCardInfo.$create(cardNumber, month, year, cvcNumber, cardType);
                    this.setCardDataToSession_(cardData);
                    return data;
                }, this);
            },

            [[Number]],
            ria.async.Future, function addCreditViaPayPal(amount){
                return this.post("Fund/AddViaPayPal", Boolean,{amount: amount});
            },

            ria.async.Future, function getCreditCardInfo(){
                var cardData = this.getCardFromSession_();
                return cardData
                    ? new ria.async.DeferredData(cardData)
                    : this.get("Fund/GetCreditCardInfo", chlk.models.funds.CreditCardInfo,{
                        needCardInfo : true
                    })
                    .then(function(data){
                        this.setCardDataToSession_(data);
                        return data;
                    }, this);
            },

            ria.async.Future, function deleteCreditCardInfo(){
                return this.post("Fund/DeleteCreditCardInfo", Boolean, { })
                    .then(function(data){
                        if(data)
                            this.setCardDataToSession_(null);
                        return data;
                    }, this);
            },

            [[chlk.models.funds.CreditCardInfo]],
            VOID, function setCardDataToSession_(creditCardData){
                this.getContext().getSession().set('creditCardData', creditCardData);
            },
            chlk.models.funds.CreditCardInfo, function getCardFromSession_(){
                return this.getContext().getSession().get('creditCardData');
            }
        ])
});