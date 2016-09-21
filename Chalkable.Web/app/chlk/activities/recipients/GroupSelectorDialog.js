REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.recipients.GroupSelectorTpl');

NAMESPACE('chlk.activities.recipients', function(){

    /**@class chlk.activities.recipients.GroupSelectorDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.recipients.GroupSelectorTpl)],
        'GroupSelectorDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.PartialUpdateRule(null, 'add-group')],
            VOID, function newCategory(tpl, model, msg_) {
                
            }
        ]);
});