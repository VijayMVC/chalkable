REQUIRE('chlk.templates.settings.AddCourseToPanoramaTpl');
REQUIRE('chlk.activities.lib.TemplateDialog');

NAMESPACE('chlk.activities.settings', function(){

    /**@class chlk.activities.settings.AddCourseToPanoramaDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.settings.AddCourseToPanoramaTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.settings.AddCourseToPanoramaTpl, null, null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'AddCourseToPanoramaDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('submit', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var ids = node.find('.course-type-select').getValue().join(',');
                node.find('.course-types-ids').setValue(ids);
            }
        ]);
});