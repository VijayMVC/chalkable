REQUIRE('chlk.templates.chlkerror.AppErrorDialogTpl');

NAMESPACE('chlk.activities.chlkerror', function () {
    /** @class chlk.activities.chlkerror.AppErrorDialog*/

    CLASS(
        [ria.mvc.ActivityGroup('AppWrapperDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.chlkerror.AppErrorDialogTpl)],
        'AppErrorDialog' , EXTENDS(chlk.activities.lib.TemplateDialog), [
        ]
    );
});
