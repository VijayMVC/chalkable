REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.AdminAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesTpl');
REQUIRE('chlk.templates.recipients.RecipientsSearchTpl');

REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');

NAMESPACE('chlk.activities.announcement', function () {
    "use strict";

    /** @class chlk.activities.announcement.AdminAnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AdminAnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttributesTpl, 'update-attributes', '.attributes-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AdminAnnouncementRecipientsTpl, 'recipients', '.recipients-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'AdminAnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage), [

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, chlk.activities.lib.DontShowLoader())],
            VOID, function addGroups(tpl, model, msg_) {
                this.dom.find('.group-ids').setValue(model.getGroupIds());
            },

            [ria.mvc.DomEventBind('change', '.recipient-search')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function searchChange(node, event, selected_){
                if(this.dom.find('[name=recipient][type=hidden]').getValue())
                    this.dom.find('.recipient-submit').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '.add-recipients .title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addRecipientsClick(node, event){
                jQuery(node.parent('.add-recipients').find('.add-recipients-btn').valueOf()).click();
            },

            [ria.mvc.DomEventBind('click', '.recipients-list')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function recipientsClick(node, event){
                if(!ria.dom.Dom(event.target).is('a') && !ria.dom.Dom(event.target).is('input'))
                    node.find('.recipient-search').trigger('focus');
            },

            [ria.mvc.DomEventBind('keypress', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    event.preventDefault();
                }
            }
        ]);

});