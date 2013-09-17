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

            [ria.mvc.DomEventBind('click', '.admin-gray')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function recipientButtonClick(node, event){
                var adminRecipients = this.getAdminRecipients();
                var recipients = adminRecipients.getRecipientButtonsInfo() || [];
                var roleId = node.getData('roleid');
                var id = node.getData('id');
                var text = node.getValue();
                var index = recipients.length;
                recipients.push(new chlk.models.announcement.AdminRecipientButton(id, roleId, text, index));
                adminRecipients.setRecipientButtonsInfo(recipients);
                this.setAdminRecipients(adminRecipients);
                this.renderRecipients();
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
                if(adminRecipients.getRecipientButtonsInfo().length)
                    this.renderRecipients()
            }
    ]);
});