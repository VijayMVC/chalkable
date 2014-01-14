
NAMESPACE('chlk.models.funds', function(){
    "use strict";

    /** @class chlk.models.funds.CreditCardTypeMapper*/

    CLASS('CreditCardTypeMapper', [

        function $(){
            BASE();
            this._creditCardMapper = {
                amex: { cssClass: 'amex-icon' },
                amazon: { cssClass: 'amazon-icon' },
                apple: { cssClass: 'apple-icon' },
                cirrus: { cssClass: 'cirrus-icon' },
                delta: { cssClass: 'delta-icon' },
                discover: { cssClass: 'discover-icon' },
                direct_debit: { cssClass: 'direct-debit-icon' },
                google: { cssClass: 'google-icon' },
                mastercard: { cssClass: 'mastercard-icon' },
                maestro: { cssClass: 'maestro-icon' },
                money_bookers: { cssClass: 'money-bookers-icon' },
                money_gram: { cssClass: 'money-gram' },
                novus: { cssClass: 'novus-icon' },
                pay_pal: { cssClass: 'pay-pal-1icon' },
                plain: { cssClass: 'plain-icon' },
                sage: { cssClass: 'sage-icon' },
                solo: { cssClass: 'solo-icon' },
                switch_card: { cssClass: 'switch-icon' },
                visa_electron: { cssClass: 'visa-electron-icon' },
                visa: { cssClass: 'visa-icon' },
                visa_debit: { cssClass: 'visa-debit-icon' },
                western_union: { cssClass: 'western-union-icon' },
                world_pay: { cssClass: 'world-pay-icon' },
                diners_club_carte_blanche: { cssClass: 'simple-card-icon' },
                diners_club_international: { cssClass: 'simple-card-icon' },
                jcb: { cssClass: 'simple-card-icon' },
                laser: { cssClass: 'simple-card-icon' }
            };
        },

        [[String]],
        Object, function map(creditCardType){
            var res = this._creditCardMapper[creditCardType];
            if(!res)
                throw new Exception('Credit Card Type doesn\'t exists');
            return res;
        },
        [[String]],
        String, function getCardCssClass(creditCardType){
            return this.map(creditCardType).cssClass;
        }
    ]);

    /** @class chlk.models.funds.CreditCardInfo*/

    CLASS('CreditCardInfo', [

        function $(){
            BASE();
            this._creditCardMapper = new chlk.models.funds.CreditCardTypeMapper();
        },

        [[String, Number, Number, Number, String]],
        function $create(cardNumber, month, year, cvcNumber, cardType){
            BASE();
            this._creditCardMapper = new chlk.models.funds.CreditCardTypeMapper();
            this.setCardNumber(cardNumber);
            this.setMonth(month);
            this.setYear(year);
            this.setCvcNumber(cvcNumber);
            this.setCardType(cardType);
        },

        [ria.serialize.SerializeProperty("cardnumber")],
        String, 'cardNumber',

        Number, 'month',
        Number, 'year',

        [ria.serialize.SerializeProperty("cvcnumber")],
        Number, 'cvcNumber',

        [ria.serialize.SerializeProperty("cardtype")],
        String, 'cardType',

        READONLY ,Object, 'mappedCardType',

        Object, function getMappedCardType(){
            var cardType = this.getCardType();
            if(cardType)
                return this._creditCardMapper.map(cardType);
            return null;
        }
    ]);
});