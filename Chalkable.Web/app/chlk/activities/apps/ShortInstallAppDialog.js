REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.ShortInstallAppTpl');
REQUIRE('chlk.templates.TransactionCompleteTpl');
REQUIRE('chlk.templates.apps.CreditCardForAppInstallTpl');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.ShortInstallAppDialog*/

    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.TransactionCompleteTpl, '', '.details-info', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.CreditCardForAppInstallTpl, '', '.details-info', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.apps.ShortInstallAppTpl)],
//        [chlk.activities.lib.ModelWaitClass('install-app-dialog-model-wait dialog-model-wait')],
//        [chlk.activities.lib.PartialUpdateClass('app-market-install')],
        'ShortInstallAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

//            [ria.mvc.DomEventBind('click', '.open-app-btn')],
//            [[ria.dom.Dom, ria.dom.Event]],
//            VOID, function openAppClick(node, event){
//                node.parent().find('.app-link').click();
//            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.common.RequestResultViewData && model.isSuccess() === true){
                    var node = this.dom.find('.install-btn');
                    node.find('.install-app-btn').addClass('x-hidden');
                    node.find('.open-app-btn').removeClass('x-hidden');
                }
            }
        ]);
});