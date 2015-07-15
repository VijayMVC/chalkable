REQUIRE('chlk.activities.announcement.BaseAnnouncementFormPage');
REQUIRE('chlk.templates.announcement.AnnouncementFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesDropdownTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');
REQUIRE('chlk.templates.announcement.Announcement');

NAMESPACE('chlk.activities.announcement', function () {

    var titleTimeout, wasTypeChanged, wasExistingTitle, wasDisabledBtn, wasDateChanged, wasTitleSaved,
        listLastTimeout;

    /** @class chlk.activities.announcement.AnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementTitleTpl, '.title-block-container', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],

        [chlk.activities.lib.PageClass('new-item')],
        'AnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage), [

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(wasTypeChanged && wasExistingTitle && wasTitleSaved)
                    this.disableSubmitBtn();
                wasTypeChanged = false;
                if(model instanceof chlk.models.announcement.LastMessages){
                    this.dom.find('#content').trigger('focus');
                    var node = this.dom.find('.no-assignments-text');
                    listLastTimeout = setTimeout(function(){
                        node.fadeOut();
                    }, 2000)
                }

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-standards-and-suggested-apps', '', ria.mvc.PartialUpdateRuleActions.Replace)],
            [[Object, Object, String]],
            VOID, function updateStandardsAndSuggestedApps(tpl, model, msg_) {
                var standardsData = new chlk.models.standard.StandardsListViewData(
                    null, model.getClassId(),
                    null, model.getStandards(),
                    model.getId()
                );
                var standardsTpl = new chlk.templates.standard.AnnouncementStandardsTpl();
                this.onPrepareTemplate_(standardsTpl, standardsData, msg_);
                standardsTpl.options({
                    ableToRemoveStandard: model.isAbleToRemoveStandard()
                });
                standardsTpl.assign(standardsData);
                standardsTpl.renderTo(this.dom.find('.standards-list').empty());

                model.setNeedButtons(true);
                model.setNeedDeleteButton(true);
                var attachmentsTpl = new chlk.templates.announcement.AnnouncementAppAttachments();
                this.onPrepareTemplate_(attachmentsTpl, model, msg_);
                attachmentsTpl.assign(model);
                attachmentsTpl.renderTo(this.dom.find('.apps-attachments-bock').empty());

                var suggestedAppsNode = this.dom.find('.suggested-apps').empty();
                if(model.getStandards() && model.getStandards().length > 0 && this.isStudyCenterEnabled()){
                    var suggestedApps = model.getSuggestedApps();
                    var suggestedAppsListData = new chlk.models.apps.SuggestedAppsList(
                        model.getClassId(),
                        model.getId(),
                        suggestedApps,
                        model.getStandards()
                    );
                    var suggestedAppsTpl = new chlk.templates.apps.SuggestedAppsListTpl();
                    this.onPrepareTemplate_(suggestedAppsTpl, model, msg_);
                    suggestedAppsTpl.assign(suggestedAppsListData);
                    suggestedAppsTpl.renderTo(suggestedAppsNode);
                    this.dom.find('.add-standards').find('.title').setText(Msg.Click_to_add_more);
                }
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

            [ria.mvc.DomEventBind('click', '.title-text')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleClick(node, event){
                var parent = node.parent('.title-block');
                parent.addClass('active');
                setTimeout(function(){
                    parent.find('input[name=title]').trigger('focus');
                }, 1)
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

            function removeDisabledClass(){
                this.dom.find('.submit-announcement')
                    .removeClass('disabled')
                    .setData('tooltip', false);
            },

            [ria.mvc.DomEventBind('click', '.submit-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function setTitleOnSubmitClick(node, event){
                if(this.dom.find('.title-text').exists())
                this.dom.find('[name-title]').setValue(this.dom.find('.title-text').getHTML());
            },

            [ria.mvc.DomEventBind('change', '.announcement-types-combo')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function typesComboClick(node, event){

            },

            [ria.mvc.DomEventBind('click', '.announcement-type-button:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function typeClick(node, event){
                if(node.hasClass('no-save'))
                    return;
                node.parent().find('.pressed').removeClass('pressed');
                node.addClass('pressed');
                var typeId = node.getData('typeid');
                var typeName = node.getData('typename');
                this.dom.find('input[name=announcementTypeId]').setValue(typeId);
                this.dom.find('input[name=announcementtypename]').setValue(typeName);
                setTimeout(function(){
                    node.parent('form').trigger('submit');
                },1);

            },

            [ria.mvc.DomEventBind('change', '#type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function typeSelect(node, event, selected_){
                var option = node.find(':selected');
                var typeId = option.getData('typeid');
                var typeName = option.getData('typename');
                this.dom.find('input[name=announcementTypeId]').setValue(typeId);
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

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, chlk.activities.lib.DontShowLoader())],
            VOID, function doSaveTitle(tpl, model, msg_) {

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

            [ria.mvc.DomEventBind('change', '#expiresdate')],
            [[ria.dom.Dom, ria.dom.Event]],
            function expiresDateChange(node, event){
                var block = this.dom.find('.title-block-container'),
                    value = node.getValue(),
                    titleNode = this.dom.find('input[name=title]');
                if(value){
                    if(!block.hasClass('with-date'))
                        block.addClass('was-empty');
                    else{
                        block.removeClass('was-empty');
                    }
                    block.addClass('with-date');
                    titleNode
                        .addClass('should-check')
                        .trigger('keyup');

                    setTimeout(function(){
                        wasDateChanged = true;
                    }, 1);
                }
                else
                    block.removeClass('with-date').removeClass('was-empty');
            },

            [ria.mvc.DomEventBind('keyup', 'input[name=title]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleKeyUp(node, event){
                wasDateChanged = false;
                var dom = this.dom, node = node, value = node.getValue();
                if(dom.find('.title-block-container').hasClass('with-date')){
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
                        var titleInput = titleBlock.find('input[name=title]');
                        titleBlock.removeClass('active');
                        var text = titleInput.getValue();
                        if(titleBlock.exists() && (titleBlock.hasClass('exists') || text == null || text == undefined || text.trim() == '')){
                            var oldText = titleInput.getData('title');
                            if(oldText != text){
                                that.updateFormByNotExistingTitle();
                                titleInput.setValue(oldText);
                                titleBlock.find('.title-text').setHTML(oldText);
                                if(!wasDisabledBtn)
                                    that.removeDisabledClass();
                            }
                        }
                        if(text && text.trim() && !dom.find('.title-block-container').hasClass('with-date')){
                            titleBlock.find('.title-text').setHTML(text);
                        }
                    }
                });

                new ria.dom.Dom().on('click.save', '.class-button[type=submit]', function($target, event){
                    if(!that.dom.find('.is-edit').getData('isedit')){
                        var classId = $target.getAttr('classId');
                        that.dom.find('input[name=classId]').setValue(classId);
                        var defaultType = $target.getData('default-announcement-type-id');
                        if(defaultType)
                            that.dom.find('input[name=announcementTypeId]').setValue(defaultType);
                    }

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
                new ria.dom.Dom().off('click.title');
                new ria.dom.Dom().off('click.save', '.class-button[type=submit]');
            }
         ]
    );
});