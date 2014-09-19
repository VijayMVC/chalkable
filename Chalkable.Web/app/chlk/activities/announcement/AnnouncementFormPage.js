REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');
REQUIRE('chlk.templates.announcement.AnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.templates.announcement.AnnouncementReminder');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.classes.TopBar');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');

NAMESPACE('chlk.activities.announcement', function () {

    var titleTimeout, wasTypeChanged, wasExistingTitle, wasDisabledBtn, wasDateChanged, wasTitleSaved;

    /** @class chlk.activities.announcement.AnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementReminder, '', '.reminders', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, 'update-attachments', '.attachments-and-applications', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementTitleTpl, '.title-block-container', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'AnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage), [

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(wasTypeChanged && wasExistingTitle && wasTitleSaved)
                    this.disableSubmitBtn();
                wasTypeChanged = false;
                if(model instanceof chlk.models.announcement.LastMessages)
                    this.dom.find('#content').trigger('focus');
            },

            [ria.mvc.DomEventBind('click', '#check-title-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkTitleClick(node, event){
                wasDisabledBtn = this.dom.find('.submit-announcement').hasClass('disabled');
                this.dom.find('.submit-announcement')
                    .addClass('disabled');
            },

            [ria.mvc.DomEventBind('keypress', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    event.preventDefault();
                }
            },

            [ria.mvc.DomEventBind('click', '.class-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function classClick(node, event){
                if(!this.dom.find('.is-edit').getData('isedit')){
                    var classId = node.getAttr('classId');
                    this.dom.find('input[name=classid]').setValue(classId);
                    var defaultType = node.getData('default-announcement-type-id');
                    if(defaultType)
                        this.dom.find('input[name=announcementtypeid]').setValue(defaultType);
                }
            },

            [ria.mvc.DomEventBind('click', '.title-text')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleClick(node, event){
                var parent = node.parent('.title-block');
                parent.addClass('active');
                setTimeout(function(){
                    parent.find('#title').trigger('focus');
                }, 1)
            },

            [ria.mvc.DomEventBind('click', '.save-title-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveClick(node, event){
                var input = this.dom.find('#title'),
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

            function removeDisabledClass(){
                this.dom.find('.submit-announcement')
                    .removeClass('disabled')
                    .setData('tooltip', false);
            },

            [ria.mvc.DomEventBind('click', '.submit-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function setTitleOnSubmitClick(node, event){
                this.dom.find('#title').setValue(this.dom.find('.title-text').getHTML());
            },

            [ria.mvc.DomEventBind('change', '.announcement-types-combo')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function typesComboClick(node, event){

            },

            [ria.mvc.DomEventBind('click', '.announcement-type-button:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function typeClick(node, event){
                node.parent().find('.pressed').removeClass('pressed');
                node.addClass('pressed');
                var typeId = node.getData('typeid');
                var typeName = node.getData('typename');
                this.dom.find('input[name=announcementtypeid]').setValue(typeId);
                this.dom.find('input[name=announcementtypename]').setValue(typeName);
            },

            [ria.mvc.DomEventBind('change', '#type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function typeSelect(node, event, selected_){
                var option = node.find(':selected');
                var typeId = option.getData('typeid');
                var typeName = option.getData('typename');
                this.dom.find('input[name=announcementtypeid]').setValue(typeId);
                this.dom.find('input[name=announcementtypename]').setValue(typeName);
                this.dom.find('#announcement-type-btn').trigger('click');
                wasTypeChanged = true;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateTitle(tpl, model, msg_) {
                if(!wasDisabledBtn || !model.isData() && wasDateChanged)
                    this.removeDisabledClass();
                var block = this.dom.find('.title-text:visible'),
                    saveBtn = this.dom.find('.save-title-btn'),
                    titleBlock = this.dom.find('.title-block');
                if(!model.isData() && this.dom.find('#title').getValue()){
                    saveBtn.setAttr('disabled', false);
                    if(block.exists() && this.dom.find('.title-block-container').hasClass('was-empty'))
                        saveBtn.trigger('click');
                    this.updateFormByNotExistingTitle();

                }else{
                    if(block.exists())
                        this.dom.find('#show-title-popup').trigger('click');
                    saveBtn.setAttr('disabled', true);
                    titleBlock.addClass('exists');
                    var titleInput = titleBlock.find('#title');
                    var text = titleInput.getValue();
                    var oldText = titleInput.getData('title');
                    if(oldText == text)
                        this.disableSubmitBtn();
                }
                wasDateChanged = false;
            },

            function disableSubmitBtn(){
                wasExistingTitle = true;
                this.dom.find('.submit-announcement')
                    .addClass('disabled')
                    .setData('tooltip', Msg.Existing_title_tooltip)
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, 'title-popup')],
            VOID, function titlePopUp(tpl, model, msg_) {
                this.dom.find('.title-text:visible').trigger('click');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, chlk.activities.lib.DontShowLoader())],
            VOID, function doSaveTitle(tpl, model, msg_) {

            },

            [ria.mvc.DomEventBind('keydown', '#title')],
            [[ria.dom.Dom, ria.dom.Event]],
            function titleKeyDown(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    var btn = this.dom.find('.save-title-btn');
                    if(!btn.getAttr('disabled'))
                        btn.trigger('click');
                    return false;
                }
            },

            [ria.mvc.DomEventBind('change', '#expiresdate')],
            [[ria.dom.Dom, ria.dom.Event]],
            function expiresDateChange(node, event){
                var block = this.dom.find('.title-block-container'),
                    value = node.getValue(),
                    titleNode = this.dom.find('#title');
                if(value){
                    if(!block.hasClass('with-date'))
                        block.addClass('was-empty');
                    else{
                        block.removeClass('was-empty');
                    }
                    block.addClass('with-date');
                    if(value != node.getData('value')){
                        titleNode.addClass('should-check');
                    }
                    titleNode.trigger('keyup');

                    setTimeout(function(){
                        wasDateChanged = true;
                    }, 1);
                }
                else
                    block.removeClass('with-date').removeClass('was-empty');
            },

            [ria.mvc.DomEventBind('keyup', '#title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleKeyUp(node, event){
                wasDateChanged = false;
                var dom = this.dom, node = node, value = node.getValue();
                if(dom.find('.title-block-container').hasClass('with-date')){
                    if(!value || !value.trim()){
                        dom.find('.save-title-btn').setAttr('disabled', true);
                    }else{
                        var picker = this.dom.find('#expiresdate');
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
                }
                node.removeClass('should-check');
            },

            [ria.mvc.DomEventBind('click', '.submit-announcement.disabled button')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function disabledSubmitClick(node, event){
                return false;
            },

            [[Boolean]],
            function updateFormByNotExistingTitle(){
                this.dom.find('.title-block').removeClass('exists');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                titleTimeout = wasTypeChanged = wasExistingTitle = wasDisabledBtn = wasDateChanged = wasTitleSaved = undefined;
                var that = this;
                new ria.dom.Dom().on('click.title', function(node, event){
                    var target = new ria.dom.Dom(event.target), dom = that.dom;
                    if(!target.parent('.title-block').exists() || target.hasClass('save-title-btn')){
                        var titleBlock = dom.find('.title-block');
                        var titleInput = titleBlock.find('#title');
                        titleBlock.removeClass('active');
                        var text = titleInput.getValue();
                        if(titleBlock.exists() && (titleBlock.hasClass('exists') || text == '' || text == null || text == undefined)){
                            var oldText = titleInput.getData('title');
                            if(oldText != text){
                                that.updateFormByNotExistingTitle();
                                titleInput.setValue(oldText);
                                titleBlock.find('.title-text').setHTML(oldText);
                                if(!wasDisabledBtn)
                                    that.removeDisabledClass();
                            }
                        }
                        if(text && !dom.find('.title-block-container').hasClass('with-date')){
                            titleBlock.find('.title-text').setHTML(text);
                        }
                    }
                });
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.title');
            }
         ]
    );
});