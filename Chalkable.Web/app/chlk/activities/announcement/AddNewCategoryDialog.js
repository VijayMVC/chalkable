REQUIRE('chlk.activities.announcement.AnnouncementEditGroupsDialog');
REQUIRE('chlk.templates.announcement.AddNewCategoryTpl');
REQUIRE('chlk.templates.announcement.LessonPlanCategoryTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddNewCategoryDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddNewCategoryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AddNewCategoryTpl, null, null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'AddNewCategoryDialog', EXTENDS(chlk.activities.announcement.AnnouncementEditGroupsDialog),[
            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanCategoryTpl)],
            VOID, function newCategory(tpl, model, msg_) {
                var newGroup = ria.dom.Dom('.new-group');
                tpl.renderTo(newGroup.setHTML(''));
                setTimeout(function(){
                    newGroup.find('.group-name').trigger('focus');
                }, 1);
            }
        ]);
});