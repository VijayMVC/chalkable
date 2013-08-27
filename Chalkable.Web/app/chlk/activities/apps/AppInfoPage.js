REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppInfo');
REQUIRE('chlk.templates.apps.AppPicture');

NAMESPACE('chlk.activities.apps', function () {


    var HIDDEN_CLASS = 'x-hidden';

    /** @class chlk.activities.apps.AppInfoPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppInfo)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'icon', '.icon', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'banner', '.banner', ria.mvc.PartialUpdateRuleActions.Replace)],
        //[ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'banner', '.banner', ria.mvc.PartialUpdateRuleActions.Replace)],

        'AppInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            function $(){
                BASE();
                this._priceCheckbox = new ria.dom.Dom('#price');
            },


            [ria.mvc.DomEventBind('click', '.price-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleAppPaymentInfo(node, event){
                var appPricing = node.parent().find('.app-pricing');
                appPricing.toggleClass(HIDDEN_CLASS, !this._priceCheckbox.is(':checked'))
            }
        ]);
});