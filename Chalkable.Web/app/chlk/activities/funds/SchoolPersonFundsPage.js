REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.funds.SchoolPersonFundsTpl');
REQUIRE('chlk.templates.funds.FundsHistoryTpl');

NAMESPACE('chlk.activities.funds', function () {

    var creditCardCssClasses = {
        amex: 'amex-icon',
        amazon: 'amazon-icon',
        apple: 'apple-icon',
        cirrus: 'cirrus-icon',
        delta: 'delta-icon',
        discover: 'discover-icon',
        direct_debit: 'direct-debit-icon',
        google: 'google-icon',
        mastercard: 'mastercard-icon',
        maestro: 'maestro-icon',
        money_bookers: 'money-bookers-icon',
        money_gram: 'money-gram',
        novus: 'novus-icon',
        pay_pal: 'pay-pal-1icon',
        plain: 'plain-icon',
        sage: 'sage-icon',
        solo: 'solo-icon',
        switch_card: 'switch-icon',
        visa_electron: 'visa-electron-icon',
        visa: 'visa-icon',
        visa_debit: 'visa-debit-icon',
        western_union: 'western-union-icon',
        world_pay: 'world-pay-icon',
        diners_club_carte_blanche: 'simple-card-icon',
        diners_club_international: 'simple-card-icon',
        jcb: 'simple-card-icon',
        laser: 'simple-card-icon'
    };

    chlk.activities.funds.BuyCreditMethods = {
        CREDIT_CARD: 0,
        PAY_PAL: 1,
        ASK_ADMIN: 2
    };

    /** @class chlk.activities.funds.SchoolPersonFundsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.funds.SchoolPersonFundsTpl)],
        'SchoolPersonFundsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._lastCardClass = null;
            },

            [ria.mvc.DomEventBind('click', '.method-container .chlk-radio-button input[type="radio"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function changeMethodClick(node, event){
                var methods_types = chlk.activities.funds.BuyCreditMethods;
                var fundForms = node.parent().parent().parent().parent().find('.funds-payment-form');
                var formClass = null;
                switch (parseInt(node.getValue())){
                    case  methods_types.CREDIT_CARD:
                        formClass = 'add-credit-card-form';
                        break;
                    case methods_types.PAY_PAL:
                        formClass = 'pay-pal-form';
                        break;
                    case methods_types.ASK_ADMIN:
                        formClass = 'send-message-form';
                        break;
                }
                fundForms.forEach(function(item){
                    if(item.hasClass(formClass))
                        item.removeClass('x-hidden');
                    else if (!item.hasClass('x-hidden'))
                        item.addClass('x-hidden');
                }.bind(this));
//
//                for(var i = 0; i < formsNodes.length; i++){
//                    if(jQuery(formsNodes[i]).hasClass(formClass)){
//                        jQuery(formsNodes[i]).removeClass('x-hidden');
//                    }
//                    if(!jQuery(formsNodes[i]).hasClass('x-hidden') && !jQuery(formsNodes[i]).hasClass(formClass)){
//                        jQuery(formsNodes[i]).addClass('x-hidden');
//                    }
//                }
            },

            [ria.mvc.DomEventBind('click', '.amount-container .chlk-radio-button input[type="radio"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function changeAmountClick(node, event){
                if(node.parent().getAttr('id') != 'amount-others'){
                    node.parent().parent().find('.chlk-radio-button')
                        .filter(function(item){
                            return item.getAttr('id') == 'amount-others'
                        }).removeClass('x-hidden');
                    node.parent().parent().find('.other-field').addClass('x-hidden');
                }else{
                    this.otherRadioBtnClick_(node);
                }

            },

            VOID, function otherRadioBtnClick_(node){
                node.parent().parent().find('.other-field').removeClass('x-hidden');
                node.parent().addClass('x-hidden');
            },

            [ria.mvc.DomEventBind('change', '#other-input-field')],
            [[ria.dom.Dom, ria.dom.Event]],
            function otherInputFieldChange(node, event){

            },

            VOID, function changeAmount_(){

            },


            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this._lastCardClass = null;
                jQuery(this.getDom().find('.add-credit-card-form .card-number').valueOf())
                    .validateCreditCard(function(result) {
                        if(result && result.card_type){
                            var creditCardForm = this.getDom().find('.add-credit-card-form');
                            var creditCardIcon = creditCardForm.find('.credit-card-icon');
                            if(creditCardIcon.hasClass('simple-card-icon')){
                                creditCardIcon.removeClass('simple-card-icon');
                            }
                            if(this._lastCardClass && creditCardIcon.hasClass(this._lastCardClass)){
                                creditCardIcon.removeClass(this._lastCardClass);
                            }
                            this._lastCardClass = creditCardCssClasses[result.card_type.name];
                            creditCardIcon.addClass(this._lastCardClass);
                        }
                }.bind(this));
            }
        ]);
});