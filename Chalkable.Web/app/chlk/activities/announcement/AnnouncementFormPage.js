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
REQUIRE('chlk.templates.apps.AppContentListTpl');

NAMESPACE('chlk.activities.announcement', function () {

    var listLastTimeout;

    /** @class chlk.activities.announcement.AnnouncementFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],

        [chlk.activities.lib.PageClass('new-item')],
        'AnnouncementFormPage', EXTENDS(chlk.activities.announcement.BaseAnnouncementFormPage), [

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(model instanceof chlk.models.announcement.LastMessages){
                    this.dom.find('#content').trigger('focus');
                    var node = this.dom.find('.no-assignments-text');
                    listLastTimeout = setTimeout(function(){
                        node.fadeOut();
                    }, 2000)
                }

            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var that = this;

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

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.save', '.class-button[type=submit]');
            },

            [ria.mvc.DomEventBind('keypress', 'input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    event.preventDefault();
                }
            },

            [ria.mvc.DomEventBind('change', '#type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function typeSelect(node, event, selected_){
                var option = node.find(':selected');
                var typeId = option.getData('typeid');
                var typeName = option.getData('typename');
                this.dom.find('input[name=announcementTypeId]').setValue(typeId);
                this.dom.find('input[name=announcementtypename]').setValue(typeName);
            },

            [ria.mvc.DomEventBind('keyup', '.max-score-container input[name="maxscore"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function maxScoreKeyUp(node, event){
                var value = node.getValue() || '';
                if(value.length == 0 || !isNaN(parseFloat(value))){
                    var checkBox = node.parent('.right-block').find('.advanced-options-container .extra-credit.checkbox');
                    var isAbleUseExtraCredit = checkBox.getData('isableuseextracredit');

                    if(!isAbleUseExtraCredit || value.length == 0 || parseFloat(value) > 0) {
                        if (!checkBox.hasAttr('disabled')) {
                            checkBox.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), false);
                            checkBox.trigger(chlk.controls.CheckBoxEvents.DISABLED_STATE.valueOf(), true);
                        }
                    }
                    else if(checkBox.hasAttr('disabled')){
                        checkBox.trigger(chlk.controls.CheckBoxEvents.DISABLED_STATE.valueOf(), false);
                    }
                }
            },

            //--------------------  STANDARDS AND SUGGESTED APPS  -----------------------

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
                    //empty recommended contens section
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

            //--------------------  APP CONTENT  --------------------------

            ArrayOf(Object), 'waitingForAppContentListRender',

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
            }
         ]
    );
});