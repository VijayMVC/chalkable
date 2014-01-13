REQUIRE('chlk.models.funds.AddCreditCardModel');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.funds', function () {

    "use strict";

    /** @class chlk.templates.funds.CreditCardTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/CreditCardInfoView.jade')],
        [ria.templates.ModelBind(chlk.models.funds.AddCreditCardModel)],
        'CreditCardTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'cardNumber',

            [ria.templates.ModelPropertyBind],
            Number, 'month',

            [ria.templates.ModelPropertyBind],
            Number, 'year',

            [ria.templates.ModelPropertyBind],
            Number, 'cvcNumber',

            [ria.templates.ModelPropertyBind],
            String, 'cardType',

            [ria.templates.ModelPropertyBind],
            Object, 'mappedCardType',

            [ria.templates.ModelPropertyBind],
            Number, 'amount',

            Boolean, function hasCardData(){
                return this.getCardNumber() && this.getCvcNumber()
                    && this.getMonth() && this.getYear();
            },

            String, function getCardCssClass(){
                var obj = this.getMappedCardType();
                if(obj && obj.cssClass)
                    return obj.cssClass;
                return 'simple-card-icon';
            },

            String, function getHiddenCreditNumber(){
                var number = this.getCardNumber();
                var arr = [];
                if(!number) return null;
                number = number.toString();
                for(var i = 0; i < number.length - 4; i++)
                    arr[i] = '*';
                return number.replace(number.substr(0, number.length - 4), arr.join(''));
            }
        ]);
});