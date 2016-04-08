REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppInfo');
REQUIRE('chlk.templates.apps.AppPicture');
REQUIRE('chlk.templates.apps.AppScreenshots');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.ApplicationStandardsTpl');

REQUIRE('chlk.activities.lib.ChlkTemplateActivity');

NAMESPACE('chlk.activities.apps', function () {


    var HIDDEN_CLASS = 'x-hidden';
    var DISABLED_CLASS = 'x-item-disabled';

    /** @class chlk.activities.apps.AppInfoPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppInfo)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.ApplicationStandardsTpl, '' , '.add-standards', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'icon', '.icon', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'banner', '.banner', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, 'externalattachpicture', '.external-attach-picture', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppScreenshots, 'screenshots', '.elem.screenshots', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppInfo, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],

        'AppInfoPage', EXTENDS(chlk.activities.lib.ChlkTemplateActivity), [

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateItem(allTpl, allModel, msg_) {

            },

            [ria.mvc.DomEventBind('change', 'input.advancedApp')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleAdvancedApp(node, event){
                var advancedStuff = this.dom.find('.advanced');
                advancedStuff.toggleClass(HIDDEN_CLASS, !node.checked());
                this.onFormChange(node, event);
            },

            [ria.mvc.DomEventBind('change', 'input.price-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleAppPaymentInfo(node, event){
                var appPricing = this.dom.find('.prices');
                appPricing.toggleClass(HIDDEN_CLASS, node.checked());
            },

            [ria.mvc.DomEventBind('change', 'input.toggle-standarts')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleStandarts(node, event){
                var addStandardsBlock = this.dom.find('.add-standards-container');
                addStandardsBlock.toggleClass(HIDDEN_CLASS, !node.checked());
                jQuery(addStandardsBlock.find('.remove-all-standards-btn').valueOf()).click();
            },

            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onFormChange(node, event){
                var submitBtnWrapper = this.dom.find('.submit-btn');
                submitBtnWrapper.addClass(DISABLED_CLASS);
                var submitBtn = submitBtnWrapper.find('button');
                submitBtnWrapper.setAttr('data-tooltip', 'Unsaved changed');
                submitBtn.setAttr('disabled', true);
                var isDraftHidden = this.dom.find('input[name=draft]');
                isDraftHidden.setValue('true');
                var updateDraftBtnWrapper = this.dom.find('.submit-draft-btn');
                updateDraftBtnWrapper.removeClass(DISABLED_CLASS);
                updateDraftBtnWrapper.removeClass("disabled");
                updateDraftBtnWrapper.removeAttr("disabled");
                var updateDraftBtn = updateDraftBtnWrapper.find('button');
                updateDraftBtn.removeAttr('disabled');
                jQuery(updateDraftBtn.valueOf()).html("Unsaved changes");
            },

            [ria.mvc.DomEventBind('click', '.close-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function picturesChanged(node, event){
                this.onFormChange(node, event);
            },

            [ria.mvc.DomEventBind('keyup', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function inputsChanged(node, event){
                this.onFormChange(node, event);
            },

            [ria.mvc.DomEventBind('change', 'input[type=checkbox], input[type=file]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkboxesChanged(node, event){
                this.onFormChange(node, event);
            },

            [ria.mvc.DomEventBind('click', '.remove-standard')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function removeStandardClick(node, event){
                this.onFormChange(node, event);
            },

            [ria.mvc.DomEventBind('change', 'input[name=schoolFlatRateEnabled]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleSchoolFlatRate(node, event){
                var schoolFlatRate = this.dom.find('.school-flat-rate');
                schoolFlatRate.toggleClass(HIDDEN_CLASS, !node.checked());
            },

            [ria.mvc.DomEventBind('change', 'input[name=classFlatRateEnabled]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleClassFlatRate(node, event){
                var classFlatRate = this.dom.find('.class-flat-rate');
                classFlatRate.toggleClass(HIDDEN_CLASS, !node.checked());
            },

            [ria.mvc.DomEventBind('click', '.add-standards .title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addStandardClick(node, event){
                jQuery(node.parent('.add-standards').find('.add-standards-btn').valueOf()).click();
            }
        ]);
});