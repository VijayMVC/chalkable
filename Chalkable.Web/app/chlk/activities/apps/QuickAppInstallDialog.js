REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.QuickAppInstallDialogTpl');
REQUIRE('chlk.templates.TransactionCompleteTpl');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.QuickAppInstallDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [chlk.activities.lib.FixedTop(80)],
        [ria.mvc.PartialUpdateRule(chlk.templates.TransactionCompleteTpl, '', '.details-info', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.apps.QuickAppInstallDialogTpl)],

        'QuickAppInstallDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.common.RequestResultViewData && model.isSuccess() === true){
                    this.dom.find('.open-app-btn').removeClass('x-hidden');
                }
            }
        ]);
});