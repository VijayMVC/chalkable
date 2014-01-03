REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.funds.SchoolPersonFundsTpl');

NAMESPACE('chlk.activities.funds', function () {

    var creditCardCssClasses = {
        amex: 'amex-icon',
        amazon: '',

        diners_club_carte_blanche: 'simple-card-icon',
        diners_club_international: 'simple-card-icon',
        discover: 'discover-icon',
        jcb: 'simple-card-icon',
        laser: 'simple-card-icon',
        visa_electron: 'visa-electron-icon',
        visa: 'visa-icon',
        mastercard: 'mastercard-icon',
        maestro: 'maestro-icon',

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
                var method = node.getValue();
                var methods_types = chlk.activities.funds.BuyCreditMethods;
                var formsNodes = node.parent().parent().parent().parent().find('.funds-payment-form').valueOf();
                var formClass = null;
                switch (parseInt(method)){
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
                for(var i = 0; i < formsNodes.length; i++){
                    if(jQuery(formsNodes[i]).hasClass(formClass)){
                        jQuery(formsNodes[i]).removeClass('x-hidden');
                    }
                    if(!jQuery(formsNodes[i]).hasClass('x-hidden') && !jQuery(formsNodes[i]).hasClass(formClass)){
                        jQuery(formsNodes[i]).addClass('x-hidden');
                    }
                }
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