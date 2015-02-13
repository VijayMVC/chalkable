REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');

REQUIRE('chlk.templates.announcement.AdminRecipients');
REQUIRE('chlk.templates.announcement.AdminAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AdminUserSearch');


NAMESPACE('chlk.activities.announcement', function(){
    "use strict";
    /**@class chlk.activities.announcement.AdminAnnouncementFormPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AdminAnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, '', '.attachments-and-applications', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AdminRecipients, '', '.recipient-part', ria.mvc.PartialUpdateRuleActions.Replace)],
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
                if(event.keyCode == ria.dom.Keys.COMMA.valueOf()){
                    this.findRecipientByName(node.getValue().trim());
                }
            },

            function findRecipientByName(text){
                var value = text.toLowerCase();
                if(value == Msg.Admin(true).toLowerCase()){
                    this.dom.find('.admin-button').trigger('click');
                }else{
                    var adminRecipients = this.getAdminRecipients();
                    var recipientsData = adminRecipients.getRecipientsData();
                    var res = {};
                    for(var p in recipientsData){
                        if(recipientsData.hasOwnProperty(p)){
                            var arr = recipientsData[p];
                            var filtered = arr.filter(function(item){
                                return item.getName().toLowerCase().trim() == text;
                            });
                            if(filtered.length){
                                res.id = filtered[0].getId();
                                res.roleId = p;
                                res.text = filtered[0].getName();
                            }
                        }
                    }
                    if(res.id)
                        this.addRecipient(res.id, parseInt(res.roleId, 10), res.text)
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
                this.onPartialRender_(this.getAdminRecipients(), chlk.activities.lib.DontShowLoader());
                this.onPartialRefresh_(this.getAdminRecipients(), chlk.activities.lib.DontShowLoader());
                setTimeout(function(){
                    this.dom.find('#super-selectbox-autocomplete').trigger('focus')
                }.bind(this), 1);
            },

            function recalculateRecipientsIds(){
                var list = this.dom.find('.super-selectbox-list'), annRecipients = [], texts = [];
                list.find('.super-selectbox-item').forEach(function(node){
                    annRecipients.push(node.getData('value') || node.getValue());
                    texts.push(node.is('select') ? node.find('option:selected').text() : node.getValue());
                });
                this.dom.find('input[name="annRecipients"]').setValue(annRecipients.join(','));
                this.dom.find('.recipients-text').setValue(texts.join(','));
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
                    this.renderRecipients();
                var dom = this.dom;
                new ria.dom.Dom().on('click.recipient', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.superboxselect') && !node.isOrInside('.buttons-container') && dom.find('input[name="annRecipients"]').getValue()){
                        dom.find('.recipients-text').show();
                        dom.find('.superboxselect').hide();
                        dom.find('.buttons-container').hide();
                    }
                });
                new ria.dom.Dom().on('click.recipient', '.recipients-text', function(node, event){
                    dom.find('.recipients-text').hide();
                    dom.find('.superboxselect').show();
                    dom.find('.buttons-container').show();
                    event.stopPropagation();
                    setTimeout(function(){
                        dom.find('#super-selectbox-autocomplete').trigger('focus')
                    }, 1);
                })
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.recipient');
                new ria.dom.Dom().off('click.recipient', '.recipients-text');
            }
    ]);
});