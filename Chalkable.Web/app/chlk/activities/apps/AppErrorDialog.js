REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');
REQUIRE('chlk.templates.common.attachments.AttachmentDialogTpl');
REQUIRE('chlk.AppApiHost');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.AppErrorDialog*/

    CLASS(
        [ria.mvc.ActivityGroup('AppWrapperDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.common.attachments.AttachmentDialogTpl)],
        'AppErrorDialog', EXTENDS(chlk.activities.common.attachments.AttachmentDialog), [
        ]);
});
