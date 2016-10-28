REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.AddNewCategoryTpl');
REQUIRE('chlk.templates.announcement.LessonPlanCategoryTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddNewCategoryDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddNewCategoryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AddNewCategoryTpl, null, null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'AddNewCategoryDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('keyup', '.category-name')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function categoryNameKeyUp(node, event){
                node.parent('.category-cnt').find('.create-category').setProp('disabled', !node.getValue() || node.getValue() == node.getData('value'));
            },

            [ria.mvc.DomEventBind('blur', '.category-name')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function categoryNameBlur(node, event){
                node.parent('.category-cnt').find('.create-category').fadeOut();
            },

            [ria.mvc.DomEventBind('focus', '.category-name')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function categoryNameFocus(node, event){
                node.parent('.category-cnt').find('.create-category').show();
            },

            [ria.mvc.DomEventBind('click', '.delete-btn:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function deleteClick(node, event){
                if(!node.hasClass('action-link'))
                    node.parent('.category-cnt').fadeOut();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.LessonPlanCategoryTpl)],
            VOID, function newCategory(tpl, model, msg_) {
                var newCategory = ria.dom.Dom('.new-category');
                tpl.renderTo(newCategory.setHTML(''));
                setTimeout(function(){
                    newCategory.find('.category-name').trigger('focus');
                }, 1);
            }
        ]);
});