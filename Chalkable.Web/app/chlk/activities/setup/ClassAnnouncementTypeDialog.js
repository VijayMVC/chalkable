REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.setup.ClassAnnouncementTypeWindowTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.ClassAnnouncementTypeDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.setup.ClassAnnouncementTypeWindowTpl)],
        'ClassAnnouncementTypeDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

        ]);
});