REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.templates.announcement.LessonPlanFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.announcement.LessonPlanSearchTpl');
REQUIRE('chlk.templates.announcement.LessonPlanCategoriesListTpl');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttributesTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');

NAMESPACE('chlk.activities.announcement', function () {

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
                    node.setData('value', node.getValue());
                }
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                var that = this;

                new ria.dom.Dom().on('click.import', function($target, event){
                    var node = ria.dom.Dom(event.target);
                    if(!node.isOrInside('.import-btn'))
                        that.dom.find('.lesson-plan-import-popup').addClass('x-hidden');
                });

                new ria.dom.Dom().off('click.import');
            }


        ]
    );
});