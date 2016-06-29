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

            [ria.mvc.DomEventBind('change', '#galleryCategoryId')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function categoryChange(node, event, selected_){
                if(node.getValue() == -1){
                    node.setValue(node.getData('value'));
                    node.trigger('chosen:updated');
                    node.parent('.left-top-container').find('.add-category-btn').trigger('click');
                }else{
                    if(this.dom.find('#public-check').checked() && node.getValue() && !node.getData('value')){
                        this.dom.find('.title-block-container').addClass('with-gallery-id');

                        if(this.dom.find('#title').getValue())
                            this.dom.find('#check-title-button').trigger('click');
                    }

                    node.setData('value', node.getValue());
                }

            },

            function checkTitle_(){
                this.dom.find('.title-block-container').addClass('with-gallery-id');
                this.dom.find('#check-title-button').trigger('click');
            },

            [ria.mvc.DomEventBind('change', '.gallery-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function addToGalleryChange(node, event, selected_){
                var select = this.dom.find('#galleryCategoryId'),
                    second = node.parent('.box-checkbox').siblings('.box-checkbox').find('.checkbox');

                second.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), !node.checked());

                if(this.dom.find('#public-check').checked())
                    this.checkTitle_();
                else{
                    this.dom.find('.title-block-container').removeClass('with-gallery-id');
                    this.removeDisabledClass();
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