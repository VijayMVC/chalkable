REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.recipients.GroupCreateTpl');

NAMESPACE('chlk.activities.recipients', function(){

    /**@class chlk.activities.recipients.GroupCreateDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.recipients.GroupCreateTpl)],
        'GroupCreateDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});