REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.AdminAnnouncementFormTpl');

REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');

NAMESPACE('chlk.activities.announcement', function () {
    "use strict";

    /** @class chlk.activities.announcement.AdminAnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AdminAnnouncementFormTpl)],
        [chlk.activities.lib.PageClass('new-item')],
        'AdminAnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage), [

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, chlk.activities.lib.DontShowLoader())],
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


            [ria.mvc.DomEventBind('click', '.save-title-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveClick(node, event){
                this.removeDisabledClass();
                setTimeout(function(){
                    node.setAttr('disabled', true);
                }, 1);
            },

            function removeDisabledClass(){
                this.dom.find('.submit-announcement')
                    .removeClass('disabled')
                    .setData('tooltip', false);
            },

            function disableSubmitBtn(){
                this.dom.find('.submit-announcement')
                    .addClass('disabled')
                    .setData('tooltip', Msg.Existing_title_tooltip)
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