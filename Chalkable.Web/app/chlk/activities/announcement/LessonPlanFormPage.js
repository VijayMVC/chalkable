REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.templates.announcement.LessonPlanFormTpl');
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

    var titleTimeout;

    /** @class chlk.activities.announcement.LessonPlanFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.LessonPlanFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttributesTpl, 'update-attributes', '.attributes-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanSearchTpl, 'categories', '#galleryCategoryForSearchContainer', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'LessonPlanFormPage', EXTENDS(chlk.activities.announcement.AnnouncementFormPage), [

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                titleTimeout = undefined;
            },

            [ria.mvc.DomEventBind('click', '.import-btn, .lesson-plan-import-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function importClick(node, event){
                this.dom.find('.lesson-plan-import-popup').toggleClass('x-hidden');
            },

            [ria.mvc.DomEventBind('change', '#in-gallery')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function addToGalleryChange(node, event, selected_){
                var select = this.dom.find('#galleryCategoryId');
                if(node.checked()){
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

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, 'addToGallery')],
            VOID, function addToGalleryRule(tpl, model, msg_) {

            },

            [ria.mvc.DomEventBind('keyup', 'input[name=title]')],
            [[ria.dom.Dom, ria.dom.Event]],
            OVERRIDE, VOID, function titleKeyUp(node, event){
                var dom = this.dom, node = node, value = node.getValue();
                if(dom.find('.title-block-container').hasClass('with-gallery-id')){
                    if(!value || !value.trim()){
                        dom.find('.save-title-btn').setAttr('disabled', true);
                    }else{
                        /*if(value == node.getData('title')){
                            this.updateFormByNotExistingTitle();
                            dom.find('.save-title-btn').setAttr('disabled', true);
                        }else{*/
                            titleTimeout && clearTimeout(titleTimeout);
                            titleTimeout = setTimeout(function(){
                                if(value == node.getValue())
                                    dom.find('#check-title-button').trigger('click');
                            }, 100);
                        //}
                    }
                }
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                var that = this;
                new ria.dom.Dom().on('click', '.create-from-template', function($target, event){
                    that.setNotSave(true);
                });
                new ria.dom.Dom().on('click.import', function($target, event){
                    var node = ria.dom.Dom(event.target);
                    if(!node.isOrInside('.import-btn'))
                        that.dom.find('.lesson-plan-import-popup').addClass('x-hidden');
                });

                new ria.dom.Dom().off('click.import');
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

            }


        ]
    );
});