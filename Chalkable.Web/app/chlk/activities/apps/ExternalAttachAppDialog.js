REQUIRE('chlk.activities.apps.AppWrapperDialog');
REQUIRE('chlk.templates.apps.ExternalAttachAppDialogTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.ExternalAttachAppDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.ExternalAttachAppDialogTpl)],
        'ExternalAttachAppDialog', EXTENDS(chlk.activities.apps.AppWrapperDialog), [

        ]);
});
