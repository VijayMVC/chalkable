REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.templates.announcement.LessonPlanFormTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.templates.announcement.LastMessages');
REQUIRE('chlk.templates.announcement.AnnouncementTitleTpl');
REQUIRE('chlk.templates.announcement.LessonPlanSearchTpl');
REQUIRE('chlk.templates.announcement.LessonPlanAutoCompleteTpl');
REQUIRE('chlk.templates.SuccessTpl');
REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.LessonPlanFormPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.LessonPlanFormTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAppAttachments, 'update-attachments', '.apps-attachments-bock', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanFormTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.AnnouncementStandardsTpl, '', '.standards-list' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, '', '.drop-down-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.SuggestedAppsListTpl, '', '.suggested-apps-block', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanSearchTpl, 'search', '.left-top-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PageClass('new-item')],
        'LessonPlanFormPage', EXTENDS(chlk.activities.announcement.AnnouncementFormPage), [
            [ria.mvc.DomEventBind('change', '#galleryCategoryForSearch')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function categorySearchChange(node, event, selected_){
                node.parent('.left-top-container').find('#changeCategoryUpdate').trigger('click');
            },

            [ria.mvc.DomEventBind('change', '#add-to-gallery')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function addToGalleryChange(node, event, selected_){
                var select = this.dom.find('#galleryCategoryId');
                if(node.checked()){
                    select.removeAttr('disabled');
                    select.setProp('disabled', false);
                }else{
                    select.setAttr('disabled', 'disabled');
                    select.setProp('disabled', true);
                }
                select.trigger('liszt:updated');
            },

            [ria.mvc.DomEventBind('change', '#galleryCategoryId')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function categorySelect(node, event, selected_){
                node.parent('.category-container').find('#add-to-galelry-btn').trigger('click')
            }
        ]
    );
});