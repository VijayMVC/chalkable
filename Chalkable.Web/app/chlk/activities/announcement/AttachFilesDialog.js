REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.FileAttachTpl');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.AttachFilesDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.FileAttachTpl)],
        'AttachFilesDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

        ]);
});