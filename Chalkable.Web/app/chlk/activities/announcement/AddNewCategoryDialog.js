REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.AddNewCategoryTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddNewCategoryDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddNewCategoryTpl)],
        'AddNewCategoryDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('submit', '.category-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            function categorySubmit(node, event){
                var node = this.dom.find('input[name=value]');
                if(!node.getValue())
                    return false;
            }
        ]);
});