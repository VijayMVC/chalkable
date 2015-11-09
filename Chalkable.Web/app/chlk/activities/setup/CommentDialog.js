REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.setup.CommentWindowTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.CommentDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.setup.CommentWindowTpl)],
        'CommentDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

        ]);
});