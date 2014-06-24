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

    var titleTimeout;

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
                this.dom.find('.title-text').setHTML(this.dom.find('#title').getValue());
                setTimeout(function(){
                    node.setAttr('disabled', true);
                }, 1);
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
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateTitle(tpl, model, msg_) {
                if(!model.isData()){
                    this.dom.find('.save-title-btn').setAttr('disabled', false);
                    this.dom.find('.title-block').removeClass('exists');
                }else{
                    this.dom.find('.save-title-btn').setAttr('disabled', true);
                    this.dom.find('.title-block').addClass('exists');
                }
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

            [ria.mvc.DomEventBind('keyup', '#title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function titleKeyUp(node, event){
                var dom = this.dom, node = node;
                if(!node.getValue() || !node.getValue().trim()){
                    dom.find('.save-title-btn').setAttr('disabled', true);
                }else{
                    if(node.getValue() == node.parent('.title-block').find('.title-text').getHTML()){
                        dom.find('.title-block').removeClass('exists');
                        dom.find('.save-title-btn').setAttr('disabled', true);
                    }else{
                        titleTimeout && clearTimeout(titleTimeout);
                        titleTimeout = setTimeout(function(){
                            dom.find('#check-title-button').trigger('click');
                        }, 100);
                    }
                }

            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var dom = this.dom;
                new ria.dom.Dom().on('click.title', function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    if(!target.parent('.title-block').exists() || target.hasClass('save-title-btn')){
                        var titleBlock = dom.find('.title-block');
                        titleBlock.removeClass('active');
                        var text = titleBlock.find('#title').getValue();
                        if(titleBlock.exists() && (titleBlock.hasClass('exists') || text == '' || text == null || text == undefined)){
                            titleBlock.removeClass('exists');
                            titleBlock.find('#title').setValue(titleBlock.find('.title-text').getHTML());
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