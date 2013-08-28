REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppInfo');
REQUIRE('chlk.templates.apps.AppPicture');

NAMESPACE('chlk.activities.apps', function () {


    var HIDDEN_CLASS = 'x-hidden';
    var DISABLED_CLASS = 'x-item-disabled';

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

            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onFormChange(node, event){
                var submitBtnWrapper = this.dom.find('.submit-btn');
                submitBtnWrapper.addClass(DISABLED_CLASS);
                var submitBtn = submitBtnWrapper.find('button');
                submitBtn.setAttr('disabled', true);



                var updateDraftBtnWrapper = this.dom.find('.submit-draft-btn');
                updateDraftBtnWrapper.removeClass(DISABLED_CLASS);
                updateDraftBtnWrapper.removeClass("disabled");
                updateDraftBtnWrapper.removeAttr("disabled");
                var updateDraftBtn = updateDraftBtnWrapper.find('button');
                updateDraftBtn.removeAttr('disabled');
            },


            [ria.mvc.DomEventBind('input', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function inputsChanged(node, event){
                this.onFormChange(node, event);
            },

            [ria.mvc.DomEventBind('change', 'input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkboxesChanged(node, event){
                this.onFormChange(node, event);
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