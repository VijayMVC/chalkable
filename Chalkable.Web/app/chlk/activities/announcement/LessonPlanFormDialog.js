REQUIRE('chlk.activities.announcement.LessonPlanFormPage');
REQUIRE('chlk.templates.announcement.LessonPlanDialogTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.announcement.LessonPlanSearchTpl');
REQUIRE('chlk.templates.announcement.LessonPlanCategoriesListTpl');
REQUIRE('chlk.templates.announcement.LessonPlanAutoCompleteTpl');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');

NAMESPACE('chlk.activities.announcement', function () {

    var titleTimeout, wasTypeChanged, wasExistingTitle, wasDisabledBtn, wasDateChanged, wasTitleSaved,
        listLastTimeout;

    /** @class chlk.activities.announcement.LessonPlanFormDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.LessonPlanDialogTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttributesTpl, 'update-attributes', '.attributes-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanSearchTpl, 'categories', '#galleryCategoryForSearchContainer', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'LessonPlanFormDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('change', '.gallery-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function addToGalleryChange(node, event, selected_){
                var select = this.dom.find('#galleryCategoryId'),
                    second = node.parent('.box-checkbox').siblings('.box-checkbox').find('.checkbox');

                second.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), !node.checked());

                if(this.dom.find('#public-check').checked()){
                    this.dom.find('.title-block-container').addClass('with-gallery-id');
                    select.removeAttr('disabled');
                    select.setProp('disabled', false);
                    this.dom.find('#check-title-btn').trigger('click');

                }else{
                    this.dom.find('.title-block-container').removeClass('with-gallery-id');
                    select.setAttr('disabled', 'disabled');
                    select.setProp('disabled', true);
                    select.find('[selected], :selected').setAttr('selected', false);
                    select.find('[selected], :selected').setProp('selected', false);
                }
                select.trigger('chosen:updated');
            },

            [ria.mvc.DomEventBind('change', '#galleryCategoryId')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function categoryChange(node, event, selected_){
                if(node.getValue() == -1){
                    node.setValue(node.getData('value'));
                    node.trigger('chosen:updated');
                    node.parent('.left-top-container').find('.add-category-btn').trigger('click');
                }else{
                    node.setData('value', node.getValue())
                }

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, 'addToGallery')],
            VOID, function addToGalleryRule(tpl, model, msg_) {

            },

            [ria.mvc.DomEventBind('keyup', 'input[name=title]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleKeyUp(node, event){
                var dom = this.dom, node = node, value = node.getValue();
                if(dom.find('.title-block-container').hasClass('with-gallery-id')){
                    if(!value || !value.trim()){
                        dom.find('.save-title-btn').setAttr('disabled', true);
                    }else{
                        titleTimeout && clearTimeout(titleTimeout);
                        titleTimeout = setTimeout(function(){
                            if(value == node.getValue())
                                dom.find('#check-title-button').trigger('click');
                        }, 100);
                    }
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanCategoriesListTpl, 'right-categories')],
            VOID, function doUpdateCategories(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('#galleryCategoryIdContainer').setHTML(''));
                setTimeout(function(){
                    var node = this.dom.find('#add-to-gallery');
                    if(!node.checked()){
                        var select = this.dom.find('#galleryCategoryId');
                        select.setAttr('disabled', 'disabled');
                        select.setProp('disabled', true);
                        select.trigger('chosen:updated');
                    }
                    if(model.getCategories().length){
                        node.removeAttr('disabled');
                        node.previous().removeAttr('disabled');
                        node.parent('.slide-checkbox').removeAttr('disabled');
                    }
                    else{
                        node.setAttr('disabled', 'disabled');
                        node.previous().setAttr('disabled', 'disabled');
                        node.parent('.slide-checkbox').setAttr('disabled', 'disabled');
                    }
                }.bind(this), 1);

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
                    listLastTimeout = setTimeout(function(){
                        node.fadeOut();
                    }, 2000)
                }

            },


            ArrayOf(Object), 'waitingForAppContentListRender',

            [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppContentListTpl, 'before-app-contents-loaded', '')],
            [[Object, Object, String]],
            VOID, function beforeAppContentsLoaded(tpl, model, msg_){
                this.onPrepareTemplate_(tpl, model, msg_);
                var appsWithContentNode = this.dom.find('.apps-with-recommended-contents');
                if(appsWithContentNode.exists() && !appsWithContentNode.hasClass('before-loading')){
                    appsWithContentNode.addClass('before-loading');
                }
            },

            [ria.mvc.PartialUpdateRule(null, 'app-contents-fail', '')],
            [[Object, Object, String]],
            VOID, function beforeAppContentsFail(tpl, model, msg_){
                this.dom.find('.apps-with-recommended-contents').removeClass('before-loading');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.apps.AppContentListTpl, 'update-app-contents', '')],
            [[Object, Object, String]],
            VOID, function updateAppContents(tpl, model, msg_){

                this.onPrepareTemplate_(tpl, model, msg_);
                tpl.assign(model);

                var appsWithContentNode = this.dom.find('.apps-with-recommended-contents');
                if(!appsWithContentNode.exists()) {
                    if(!this.waitingForAppContentListRender)
                        this.waitingForAppContentListRender = [];

                    this.waitingForAppContentListRender.push({
                        tpl: tpl,
                        appId: model.getApplication().getId()
                    })
                }
                else {

                    this.renderAppContentList_(tpl, model.getApplication().getId())
                }
            },


            [[chlk.templates.apps.AppContentListTpl, chlk.models.id.AppId]],
            VOID, function renderAppContentList_(tpl, appId){

                var appsWithContentNode = this.dom.find('.apps-with-recommended-contents');
                if(appsWithContentNode.exists()){
                    appsWithContentNode.removeClass('before-loading');
                    var appId = appId.valueOf();
                    var resNode = appsWithContentNode.find('.app-contents-block[data-appId=' + appId + ']');

                    resNode = resNode.exists() ? resNode.empty() : appsWithContentNode;
                    var dom = new ria.dom.Dom().fromHTML(tpl.render());
                    dom.appendTo(resNode);
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
                    this.suggestedAppsPartialUpdate_(model, suggestedAppsNode, msg_);
                    this.dom.find('.add-standards').find('.title').setText(Msg.Click_to_add_more);
                }
                else {
                    this.dom.find('.apps-with-recommended-contents').empty();
                }
            },

            [[chlk.models.announcement.FeedAnnouncementViewData, ria.dom.Dom, String]],
            VOID, function suggestedAppsPartialUpdate_(model, suggestedAppsNode, msg_){
                var suggestedApps = model.getSuggestedApps();
                var suggestedAppsListData = new chlk.models.apps.SuggestedAppsList(
                    model.getClassId(),
                    model.getId(),
                    suggestedApps,
                    model.getStandards(),
                    null,
                    model.getType()
                );
                var suggestedAppsTpl = new chlk.templates.apps.SuggestedAppsListTpl();
                this.onPrepareTemplate_(suggestedAppsTpl, model, msg_);
                suggestedAppsTpl.assign(suggestedAppsListData);
                suggestedAppsTpl.renderTo(suggestedAppsNode);
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

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);

                if(this.waitingForAppContentListRender){
                    this.waitingForAppContentListRender.forEach(function(_){
                        this.renderAppContentList_(_.tpl, _.appId);
                    }, this);

                    this.waitingForAppContentListRender = [];
                }

            },

            OVERRIDE, VOID, function onStop_() {
                var button = new ria.dom.Dom('#save-form-button');
                if(button.exists())
                    button.trigger('click');

                new ria.dom.Dom().off('click.title');
                new ria.dom.Dom().off('click.save', '.class-button[type=submit]');

                BASE();

            },


            [ria.mvc.DomEventBind('click', '.calendar-mark')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function dateMarkClick(node, event){
                node.parent().find('.hasDatepicker').trigger('focus');
            },

            [ria.mvc.DomEventBind('submit', '.announcement-form>FORM')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){

                var that = this;

                this.saveAttributes_();

                setTimeout(function(){
                    var submitType = node.getData('submit-type');
                    if(submitType == "submitOnEdit" || submitType == "submit"){
                        that.dom.find('#save-form-button').remove();
                        node.setData('submit-type', null);
                    }
                }, 10);

            },

            [ria.mvc.DomEventBind('click', '.add-loader-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addLoaderOnSubmitClick(node, event){
                this.dom.find('#save-form-button').remove();
            },

            [ria.mvc.DomEventBind('click', '.add-standards .title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addStandardsClick(node, event){
                jQuery(node.parent('.add-standards').find('.add-standards-btn').valueOf()).click();
            },

            function saveAttributes_(){
                var attrDomElems = new ria.dom.Dom('.attributes-block').find('.table') || [];
                var attrs = [];

                var that = this;

                attrDomElems.forEach(function(domElem){
                    var id = domElem.getAttr('id').replace('assigned-attr-', '');
                    var attributeTypeId = that.dom.find('#announcement-attrs-type-'+id).getValue();
                    var name = that.dom.find('#announcement-attrs-type-'+id).find('option:selected').getText();
                    var isVisible = !that.dom.find('input[name=attr-hidefromstudents-'+id + "]").checked();
                    var text = that.dom.find('#text-'+id).getValue();
                    var announcementType = domElem.getAttr('data-announcement-type');
                    var announcementRef = domElem.getAttr('data-announcement-id');
                    var sisAttrId = domElem.getAttr('data-sis-assigned-attr-id');

                    var attachmentModel = null;
                    var attachmentDom = that.dom.find('#file-attribute-attachment-' + id);
                    if (attachmentDom.valueOf()){
                        var attachmentId = attachmentDom.getAttr('data-attachment-id');
                        var stiAttachment = attachmentDom.getAttr('data-sti-attachment') == 'true';
                        var attachmentUuid = attachmentDom.getAttr('data-uuid') || '';
                        var attachmentName = attachmentDom.getAttr('data-name') || '';
                        var attachmentMimeType = attachmentDom.getAttr('data-mime-type') || '';

                        var attachmentObj = {
                            id: attachmentId,
                            stiattachment: stiAttachment,
                            uuid: attachmentUuid,
                            name: attachmentName,
                            mimetype: attachmentMimeType
                        };

                        attachmentModel = chlk.models.announcement.AnnouncementAttributeAttachmentViewData.$fromObject(attachmentObj);
                    }

                    var attr = {
                        id: id,
                        text: text,
                        name: name,
                        visibleforstudents: isVisible,
                        attributetypeid: attributeTypeId || 1,
                        announcementref: announcementRef,
                        announcementtype: announcementType,
                        sisactivityassignedattributeid: sisAttrId
                    };

                    var attrViewData = chlk.models.announcement.AnnouncementAttributeViewData.$fromObject(attr);
                    attrViewData.setAttributeAttachment(attachmentModel);
                    attrs.push(attrViewData.getPostData());
                });


                var attrJson = JSON.stringify(attrs);
                this.dom.find('input[name=announcementAssignedAttrs]').setValue(attrJson);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.common.SimpleObjectTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateApp(tpl, model, msg_) {
                this.dom.find('.attach-icon[data-id=' + model.getValue() + ']').removeClass('x-hidden');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttributeTpl)],
            VOID, function updateAttribute(tpl, model, msg_) {

                if (msg_){
                    if (msg_ == 'remove-attribute'){
                        this.dom.find('#assigned-attr-' + model.getId()).removeSelf();
                        if(!this.dom.find('.attribute-item').count()){
                            this.dom.find('.attributes-attach-area').addClass('x-hidden');
                            this.dom.find('.main-attach-attribute-btn').removeClass('x-hidden');
                        }
                    }
                    if (msg_ == 'add-attribute'){
                        var attrDom = new ria.dom.Dom().fromHTML(tpl.render());
                        attrDom.appendTo('.attributes-block');
                        this.dom.find('.attributes-attach-area').removeClass('x-hidden');
                        this.dom.find('.main-attach-attribute-btn').addClass('x-hidden');
                    }

                    if (msg_ == 'add-attribute-attachment'){

                        var attachmentTpl = chlk.templates.announcement.AnnouncementAttributeAttachmentTpl();
                        attachmentTpl.assign(model.getAttributeAttachment());
                        var attrAttachmentDom = new ria.dom.Dom().fromHTML(attachmentTpl.render());
                        attrAttachmentDom.appendTo('#file-attribute-attachment-area-' + model.getId());

                        var parent = attrAttachmentDom.parent('.description');
                        parent.addClass('with-file');

                        var attachDocBtnTpl = chlk.templates.announcement.AnnouncementAttributeAttachDocBtnTpl();
                        attachDocBtnTpl.assign(model);
                        var attachDocBtnDom = new ria.dom.Dom().fromHTML(attachDocBtnTpl.render());
                        new ria.dom.Dom('#file-attachment-button-'+ model.getId()).empty();
                        attachDocBtnDom.appendTo('#file-attachment-button-'+ model.getId());

                        autosize.update(parent.find('textarea').valueOf()[0]);
                    }

                    if (msg_ == 'remove-attribute-attachment'){
                        if (model.getAttributeAttachment() == null){
                            var el = this.dom.find('#file-attribute-attachment-' + model.getId()),
                                parent = el.parent('.with-file');
                            parent.removeClass('with-file');
                            el.removeSelf();
                            var attachDocBtnTpl = chlk.templates.announcement.AnnouncementAttributeAttachDocBtnTpl();
                            attachDocBtnTpl.assign(model);
                            var attachDocBtnDom = new ria.dom.Dom().fromHTML(attachDocBtnTpl.render());
                            new ria.dom.Dom('#file-attachment-button-'+ model.getId()).empty();
                            attachDocBtnDom.appendTo('#file-attachment-button-'+ model.getId());

                            autosize.update(parent.find('textarea').valueOf()[0]);
                        }
                    }
                }
            }
        ]
    );
});