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
        //[ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'screenshot', '.screenshots', ria.mvc.PartialUpdateRuleActions.Replace)],

        'AppInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            function $(){
                BASE();
            },


            [ria.mvc.DomEventBind('click', 'input.price-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleAppPaymentInfo(node, event){
                var appPricing = this.dom.find('.prices');
                appPricing.toggleClass(HIDDEN_CLASS, node.checked());
            },

            [ria.mvc.DomEventBind('click', 'input[name=schoolFlatRateEnabled]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolFlatRate(node, event){
                var schoolFlatRate = this.dom.find('.school-flat-rate');
                schoolFlatRate.toggleClass(HIDDEN_CLASS, !node.checked());
            },

            [ria.mvc.DomEventBind('click', 'input[name=classFlatRateEnabled]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleClassFlatRate(node, event){
                var classFlatRate = this.dom.find('.class-flat-rate');
                classFlatRate.toggleClass(HIDDEN_CLASS, !node.checked());
            }
        ]);
});