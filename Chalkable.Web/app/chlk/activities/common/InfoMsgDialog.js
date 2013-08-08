REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.common.InfoMsg');

NAMESPACE('chlk.activities.common', function () {
     /** @class chlk.activities.common.InfoMsg */
     CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.common.InfoMsg)],
        'InfoMsgDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
