REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.AdminAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesTpl');

REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');

NAMESPACE('chlk.activities.announcement', function () {
    "use strict";

    var titleTimeout, wasTypeChanged, wasExistingTitle, wasDisabledBtn, wasDateChanged, wasTitleSaved,
        listLastTimeout;

    /** @class chlk.activities.announcement.AdminAnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AdminAnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttributesTpl, 'update-attributes', '.attributes-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AdminAnnouncementRecipientsTpl, 'recipients', '.recipients-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'AdminAnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage), [

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateTitle(tpl, model, msg_) {
                if(!wasDisabledBtn || !model.isData() && wasDateChanged)
                    this.removeDisabledClass();
                var block = this.dom.find('.title-text:visible'),
                    saveBtn = this.dom.find('.save-title-btn'),
                    titleBlock = this.dom.find('.title-block');
                if(!model.isData() && this.dom.find('input[name=title]').getValue()){
                    saveBtn.setAttr('disabled', false);
                    if(block.exists() && this.dom.find('.title-block-container').hasClass('was-empty'))
                        saveBtn.trigger('click');
                    this.updateFormByNotExistingTitle();

                }else{
                    if(block.exists())
                        this.dom.find('#show-title-popup').trigger('click');
                    saveBtn.setAttr('disabled', true);
                    titleBlock.addClass('exists');
                    var titleInput = titleBlock.find('input[name=title]');
                    var text = titleInput.getValue();
                    var oldText = titleInput.getData('title');
                    if(oldText == text)
                        this.disableSubmitBtn();
                }
                wasDateChanged = false;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttributesTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doSaveTitle(tpl, model, msg_) {

            },

            [ria.mvc.DomEventBind('keyup', 'input[name=title]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleKeyUp(node, event){
                wasDateChanged = false;
                var dom = this.dom, node = node, value = node.getValue();
                if(!value || !value.trim()){
                    dom.find('.save-title-btn').setAttr('disabled', true);
                }else{
                    if(value == node.getData('title') && !node.hasClass('should-check')){
                        this.updateFormByNotExistingTitle();
                        dom.find('.save-title-btn').setAttr('disabled', true);
                    }else{
                        titleTimeout && clearTimeout(titleTimeout);
                        titleTimeout = setTimeout(function(){
                            if(value == node.getValue())
                                dom.find('#check-title-button').trigger('click');
                        }, 100);
                    }
                }

                node.removeClass('should-check');
            },

            [[Boolean]],
            function updateFormByNotExistingTitle(){
                this.dom.find('.title-block').removeClass('exists');
            },

            [ria.mvc.DomEventBind('keydown', 'input[name=title]')],
            [[ria.dom.Dom, ria.dom.Event]],
            function titleKeyDown(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    var btn = this.dom.find('.save-title-btn');
                    if(!btn.getAttr('disabled'))
                        btn.trigger('click');
                    return false;
                }
            },

            function disableSubmitBtn(){
                wasExistingTitle = true;
                this.dom.find('.submit-announcement')
                    .addClass('disabled')
                    .setData('tooltip', Msg.Existing_title_tooltip)
            },

            [ria.mvc.DomEventBind('click', '.save-title-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveClick(node, event){
                var input = this.dom.find('input[name=title]'),
                    value = input.getValue();
                this.dom.find('.title-text').setHTML(value);
                input.setData('title', value);
                this.removeDisabledClass();
                wasTitleSaved = true;
                wasExistingTitle = false;
                setTimeout(function(){
                    node.setAttr('disabled', true);
                }, 1);
            },

            [ria.mvc.DomEventBind('click', '#check-title-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkTitleClick(node, event){
                wasDisabledBtn = this.dom.find('.submit-announcement').hasClass('disabled');
                this.dom.find('.submit-announcement')
                    .addClass('disabled');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, chlk.activities.lib.DontShowLoader())],
            VOID, function addGroups(tpl, model, msg_) {
                this.dom.find('.group-ids').setValue(model.getGroupIds());
            },

            [ria.mvc.DomEventBind('click', '.add-recipients .title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addRecipientsClick(node, event){
                jQuery(node.parent('.add-recipients').find('.add-recipients-btn').valueOf()).click();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(wasTypeChanged && wasExistingTitle && wasTitleSaved)
                    this.disableSubmitBtn();
                wasTypeChanged = false;
                if(model instanceof chlk.models.announcement.LastMessages){
                    this.dom.find('#content').trigger('focus');
                    var node = this.dom.find('.no-assignments-text');
                    var listLastTimeout = setTimeout(function(){
                        node.fadeOut();
                    }, 2000)
                }
            },

            [ria.mvc.DomEventBind('keypress', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    event.preventDefault();
                }
            },

            function removeDisabledClass(){
                this.dom.find('.submit-announcement')
                    .removeClass('disabled')
                    .setData('tooltip', false);
            },

            [ria.mvc.DomEventBind('click', '.submit-announcement.disabled button')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function disabledSubmitClick(node, event){
                return false;
            },


            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var that = this;
                new ria.dom.Dom().on('click.save', '.class-button[type=submit]', function($target, event){

                    if($target.getAttr('type') == 'submit'){
                        var $form = that.dom.find('form');
                        $form.setData('submit-name', $target.getAttr('name'));
                        $form.setData('submit-value', $target.getValue() || $target.getAttr('value'));
                        $form.setData('submit-skip', $target.hasClass('validate-skip'));
                        $form.trigger('submit');
                    }
                });
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.save', '.class-button[type=submit]');
            }
        ]);

});