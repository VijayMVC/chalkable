REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');

REQUIRE('chlk.templates.announcement.AdminRecipients');
REQUIRE('chlk.templates.announcement.AdminAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.AnnouncementReminder');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AdminUserSearch');


NAMESPACE('chlk.activities.announcement', function(){
    "use strict";
    /**@class chlk.activities.announcement.AdminAnnouncementFormPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AdminAnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementReminder, '', '.reminders', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, '', '.attachments-and-applications', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AdminRecipients, '', '.super-selectbox-list', ria.mvc.PartialUpdateRuleActions.Replace)],
//        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AdminAnnouncementFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'AdminAnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage),[

            chlk.models.announcement.AdminRecipients, 'adminRecipients',

            chlk.templates.announcement.AdminRecipients, 'adminRecipientsTpl',

            Number, 'recipientsLength',

            [ria.mvc.DomEventBind('click', '.admin-gray')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function recipientButtonClick(node, event){
                var roleId = node.getData('roleid');
                var id = node.getData('id');
                var text = node.getValue();
                this.addRecipient(id, roleId, text);
            },

            [ria.mvc.DomEventBind('click', '.superboxselect')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function recipientBlockClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(target.is('.superboxselect') ||
                    target.is('.super-selectbox-list') ||
                    target.is('.super-selectbox-input') ||
                    target.is('.super-selectbox-li'))
                        this.dom.find('#super-selectbox-autocomplete').trigger('focus')
            },

            [ria.mvc.DomEventBind('keydown', '#super-selectbox-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function autoCompleteKeyDown(node, event){
                var len = this.getRecipientsLength();
                if(len){
                    if(event.keyCode == ria.dom.Keys.BACKSPACE.valueOf() && this.getRecipientsLength() && !node.getValue()){
                        event.preventDefault();
                        this.removeRecipient(len - 1);
                    }
                    if(len > 1)
                        switch (event.keyCode){
                            case ria.dom.Keys.LEFT.valueOf():
                                this.dom.find('.super-selectbox-focus[data-index="' + (len - 2) + '"]').trigger('focus');break;
                            case ria.dom.Keys.RIGHT.valueOf():
                                this.dom.find('.super-selectbox-focus[data-index="0"]').trigger('focus');break;
                        }
                }
            },

            [ria.mvc.DomEventBind('keydown', '.super-selectbox-focus')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function focusedKeyDown(node, event){
                var index = node.getData('index'), len = this.getRecipientsLength();
                switch (event.keyCode){
                    case ria.dom.Keys.LEFT.valueOf(): var lIndex = index > 0 ? index - 1 : len - 1;
                        this.dom.find('.super-selectbox-focus[data-index="' + lIndex + '"]').trigger('focus');break;
                    case ria.dom.Keys.RIGHT.valueOf(): var rIndex = index < len - 1 ? index + 1 : 0;
                        this.dom.find('.super-selectbox-focus[data-index="' + rIndex + '"]').trigger('focus');break;
                    case ria.dom.Keys.BACKSPACE.valueOf(): this.removeRecipient(index);event.preventDefault();break;
                }
                if(event.keyCode >= 48){
                    node.addClass('white-input');
                    setTimeout(function(){
                        var value = node.getValue();
                        if(value){
                            this.dom.find('#super-selectbox-autocomplete').setValue(value).trigger('focus');
                            node.setValue('');
                        }
                        node.removeClass('white-input');
                    }.bind(this), 1)
                }
            },

            function removeRecipient(index){
                var adminRecipients = this.getAdminRecipients();
                var recipients = adminRecipients.getRecipientButtonsInfo() || [];
                this.setRecipientsLength(recipients.length - 1);
                recipients.splice(index, 1);
                this.saveRecipients(adminRecipients, recipients);
            },

            function saveRecipients(adminRecipients, recipients){
                adminRecipients.setRecipientButtonsInfo(recipients);
                this.setAdminRecipients(adminRecipients);
                this.renderRecipients();
                setTimeout(function(){
                    this.dom.find('#super-selectbox-autocomplete').trigger('focus')
                }.bind(this), 1);
            },

            function addRecipient(id, roleId, text){
                var adminRecipients = this.getAdminRecipients();
                var recipients = adminRecipients.getRecipientButtonsInfo() || [];
                var index = recipients.length;
                this.setRecipientsLength(index + 1);
                recipients.push(new chlk.models.announcement.AdminRecipientButton(id, roleId, text, index));
                this.saveRecipients(adminRecipients, recipients);
            },

            [ria.mvc.DomEventBind('keydown', '#super-selectbox-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function recipientEnterClick(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER.valueOf()) {
                    event.preventDefault();
                }
            },

            [ria.mvc.DomEventBind('change', '#super-selectbox-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function autoCompleteChange(node, event, selected_){
                var id = this.dom.find('#super-selectbox-autocomplete-hidden').getValue();
                if(id)
                    this.addRecipient('0|-1|-1|' + id, null, node.getValue());
            },

            [ria.mvc.DomEventBind('change', '.super-selectbox-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function recipientSelectChange(node, event, selected_){
                var index = node.getData('index');
                var adminRecipients = this.getAdminRecipients();
                var recipients = adminRecipients.getRecipientButtonsInfo();
                recipients[index].setId(node.getValue());
                adminRecipients.setRecipientButtonsInfo(recipients);
                this.setAdminRecipients(adminRecipients);
                this.recalculateRecipientsIds();
            },

            function renderRecipients(){
                this.onPartialRender_(this.getAdminRecipients(), window.noLoadingMsg);
                this.onPartialRefresh_(this.getAdminRecipients(), window.noLoadingMsg);
            },

            function recalculateRecipientsIds(){
                var list = this.dom.find('.super-selectbox-list'), annRecipients = [];
                list.find('.super-selectbox-item').forEach(function(node){
                    annRecipients.push(node.getData('value') || node.getValue());
                });
                this.dom.find('input[name="annRecipients"]').setValue(annRecipients.join(','));
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                if(!this.getAdminRecipientsTpl())
                    this.setAdminRecipientsTpl(new chlk.templates.announcement.AdminRecipients);
                var adminRecipients = model.getAdminRecipients();
                this.setAdminRecipients(model.getAdminRecipients());
                var len = adminRecipients.getRecipientButtonsInfo().length;
                this.setRecipientsLength(len);
                if(len)
                    this.renderRecipients()
            }
    ]);
});