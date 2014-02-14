REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.BaseAnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.AnnouncementReminder');
REQUIRE('chlk.templates.announcement.LastMessages');

NAMESPACE('chlk.activities.announcement', function () {
    "use strict";

    /** @class chlk.activities.announcement.BaseAnnouncementFormPage*/
    CLASS(
        'BaseAnnouncementFormPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._handler = null;
            },

            [ria.mvc.DomEventBind('click', '#add-reminder')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addReminderClick(node, event){
                this.dom.find('.new-reminder').removeClass('x-hidden');
            },

            [ria.mvc.DomEventBind('change', '#expiresdate')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function dueDateChange(node, event){
                var dt = new Date(node.getValue());
                if(!dt.valueOf())
                    node.setValue('');
            },

            [ria.mvc.DomEventBind('click', '#reminders-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function showRemindersClick(node, event){
                this.dom.find('.reminders-container').toggleClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formClick(node, event){

            },

            [ria.mvc.DomEventBind('click', '.remove-reminder')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function removeReminderClick(node, event){
                setTimeout(function(){
                    node.parent('.reminder').remove();
                }, 10);
            },

            [ria.mvc.DomEventBind('submit', '.announcement-form>FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){

                var that = this;
                setTimeout(function(){
                    var submitType = node.getData('submit-type');
                    if(submitType == "submitOnEdit" || submitType == "submit"){
                        that.dom.find('#save-form-button').remove();
                        that.addPartialRefreshLoader();
                        node.setData('submit-type', null);
                    }
                }, 10);

            },

            [ria.mvc.DomEventBind('click', '.add-loader-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addLoaderOnSubmitClick(node, event){
                this.addPartialRefreshLoader();
                this.dom.find('#save-form-button').remove();
            },

            [ria.mvc.DomEventBind('change', '.reminder-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function inputChange(node, event){
                this.addEditReminder(node);
            },

            /*[ria.mvc.DomEventBind('click', '#content')],
             [[ria.dom.Dom, ria.dom.Event]],
             VOID, function focusContent(node, event){
             node.triggerEvent('focus');
             },*/
            [ria.mvc.DomEventBind('click', '.add-standards .title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addStandardsClick(node, event){
                jQuery(node.parent('.add-standards').find('.add-standards-btn').valueOf()).click();
            },

            [ria.mvc.DomEventBind('click', '.advanced-options-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function advancedOptionsButtonClick(node, event){
                var parentNode = node.parent('.advanced-options');
                node.toggleClass('selected');
                parentNode.find('.separator').toggleClass('x-hidden');
                parentNode.find('.advanced-options-container').toggleClass('x-hidden');
            },


            [ria.mvc.DomEventBind('focus keydown keyup', '#content')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function showDropDown(node, event){
                if(this.dom.find('[name=announcementtypeid]').getValue() != chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf()){
                    var dropDown = this.dom.find('.new-item-dropdown');
                    if(!node.getValue() && !dropDown.is(':visible')){
                        this.dom.find('#list-last-button').trigger('click');
                    }else{
                        if(event.type == 'keydown'){
                            var isUp = event.keyCode == ria.dom.Keys.UP.valueOf(),
                                isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                            if(dropDown.is(':visible') && (isUp || isDown)){
                                var current = dropDown.find('.current');
                                var currentIndex = parseInt(current.getData('index'),10), nextItem;
                                var moveUp = currentIndex > 0 && isUp;
                                var moveDown = currentIndex < (dropDown.getData('length') - 1) && isDown;
                                if(moveUp || moveDown){
                                    current.removeClass('current');
                                    if(moveUp){
                                        currentIndex--;
                                    }else{
                                        currentIndex++;
                                    }
                                    nextItem = dropDown.find('.autofill-item[data-index="' + currentIndex + '"]').addClass('current');
                                    node.setValue(nextItem.getData('value'));
                                }
                            }else{
                                dropDown.addClass('x-hidden');
                                //if(event.keyCode == ria.dom.Keys.ENTER.valueOf())
                                    //return false;
                            }
                        }
                    }
                }
                return true;
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                if(model.getClass() == chlk.models.announcement.LastMessages){
                    this.dom.find('#content').trigger('focus');
                }

                if(model.getClass() == chlk.models.announcement.Reminder){
                    var parent = this.dom.find('.new-reminder');
                    var input = parent.find('.reminder-input');
                    var select = parent.find('.reminder-select');
                    input.setAttr('disabled', false).setValue('');
                    select.setAttr('disabled', false)
                        .find('option[value=1]').setAttr('selected', true)
                        .find('option[value=7]').setAttr('selected', false);
                    parent.find('.remove-reminder').removeClass('x-hidden');
                    jQuery(select.valueOf()).trigger('liszt:updated');
                    parent.addClass('x-hidden');
                }
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                var that = this;
                this._handler = function(event, node){
                    var target = new ria.dom.Dom(event.target);
                    if(!target.parent('.drop-down-container').exists() && target.getAttr('name') != 'content')
                        that.dom.find('.new-item-dropdown').addClass('x-hidden');
                };
                jQuery(this.dom.valueOf()).on('change', '.reminder-select', function(){
                    that.addEditReminder(new ria.dom.Dom(this));
                });
                new ria.dom.Dom().on('click.dropdown', this._handler);
            },

            OVERRIDE, VOID, function onStop_() {
                var button = new ria.dom.Dom('#save-form-button');
                if(button.exists())
                    button.trigger('click');
                new ria.dom.Dom().off('click.dropdown', this._handler);
                BASE();
            },

            [[ria.dom.Dom]],
            VOID, function addEditReminder(node) {
                var parent = node.parent('.reminder');
                var input = parent.find('.reminder-input');
                var inputValue = input.getValue();
                if(inputValue){
                    var select = parent.find('.reminder-select');
                    var id = parent.getData('id');
                    var form = this.dom.find('#add-edit-reminder');
                    var reminders = form.getData('reminders'), isDuplicate = false;
                    var before = parseInt(select.getValue(), 10) * parseInt(inputValue, 10);
                    isDuplicate = reminders.indexOf(before) > -1;
                    if(!isDuplicate){
                        reminders.push(before);
                        form.setData('reminders', reminders);

                        if(!id){
                            input.setAttr('disabled', true);
                            select.setAttr('disabled', true);
                            parent.find('.remove-reminder').addClass('x-hidden');
                            jQuery(select.valueOf()).trigger('liszt:updated');
                        }
                    }
                    form.find('[name=duplicate]').setValue(isDuplicate);
                    form.find('[name=id]').setValue(id || '');
                    form.find('[name=before]').setValue(before);
                    form.trigger('submit');
                }

            }
        ]
    );
});