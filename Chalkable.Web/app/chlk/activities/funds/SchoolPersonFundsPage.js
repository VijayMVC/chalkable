REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.funds.SchoolPersonFundsTpl');
REQUIRE('chlk.templates.funds.FundsHistoryTpl');
REQUIRE('chlk.templates.funds.CreditCardTpl');

NAMESPACE('chlk.activities.funds', function () {

    chlk.activities.funds.BuyCreditMethods = {
        CREDIT_CARD: 0,
        PAY_PAL: 1,
        ASK_ADMIN: 2
    };

    /** @class chlk.activities.funds.SchoolPersonFundsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.funds.SchoolPersonFundsTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.funds.SchoolPersonFundsTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.funds.CreditCardTpl, '', '.add-credit-card-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.funds.FundsHistoryTpl, '', '.funds-history-grid', ria.mvc.PartialUpdateRuleActions.Replace)],
        'SchoolPersonFundsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._lastCardClass = null;
                this._creditCardMapper = new chlk.models.funds.CreditCardTypeMapper();
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
            },

            [ria.mvc.DomEventBind('click', '.amount-container .chlk-radio-button input[type="radio"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function changeAmountBtnClick(node, event){
                if(node.parent().getAttr('id') != 'amount-others'){
                    node.parent().parent().find('.chlk-radio-button')
                        .filter(function(item){
                            return item.getAttr('id') == 'amount-others'
                        }).removeClass('x-hidden');
                    node.parent().parent().find('.other-field').addClass('x-hidden');
                    this.changeAmount_(parseInt(node.getValue()));
                }else{
                    this.otherRadioBtnClick_(node);
                }
            },

            [[ria.dom.Dom]],
            VOID, function otherRadioBtnClick_(node){
                node.parent().parent().find('.other-field').removeClass('x-hidden');
                node.parent().addClass('x-hidden');
            },

            [[Number]],
            VOID, function changeAmount_(amount){
                this.getDom().find('.funds-payment-form [name="amount"]')
                    .forEach(function(item){ item.setValue(amount)});
                this.getDom().find('.funds-payment-form .amount-btn-text')
                    .forEach(function(item){
                        jQuery(item.valueOf()).text("$" + amount);
                    });
            },

            [ria.mvc.DomEventBind('change', '#other-input-field')],
            [[ria.dom.Dom, ria.dom.Event]],
            function otherInputFieldChange(node, event){
                this.changeAmount_(parseInt(node.getValue()));
            },

//            [ria.mvc.DomEventBind('click', '.saved-card-form .close-btn')],
//            [[ria.dom.Dom, ria.dom.Event]],
//            function closeBtnClick(node, event){
//                node.parent().addClass('x-hidden');
//                node.parent().parent().find('.edit-form').removeClass('x-hidden');
//            },

            [ria.mvc.DomEventBind('click', '.funds-transaction-result-view .redirect-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            function tryAgainClick(node, event){
                this.getDom().find('.action-forms').removeClass('x-hidden');
                this.getDom().find('.funds-transaction-result-view').addClass('x-hidden');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.onPartialRender_(model.getAddCreditCardData(), chlk.activities.lib.DontShowLoader());
                this.onPartialRefresh_(model.getAddCreditCardData(), chlk.activities.lib.DontShowLoader());

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
                            this._lastCardClass =  this._creditCardMapper.getCardCssClass(result.card_type.name);
                            creditCardIcon.addClass(this._lastCardClass);
                            creditCardForm.find('[name="cardtype"]').setValue(result.card_type);
                        }
                }.bind(this));
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                if(model.getClass() == chlk.models.funds.AddCreditCardModel){
                    var dom = this.getDom();
                    var amount = parseInt(dom.find('.amount-container input[checked="checked"]').getValue());
                    model.setAmount(amount);
                    this.changeAmount_(amount);
                }
            }
        ]);
});