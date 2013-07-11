REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.bgtasks.BgTaskLogs');

NAMESPACE('chlk.activities.bgtasks', function () {

    /** @class chlk.activities.bgtasks.BgTaskLogListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.bgtasks.BgTaskLogs)],
        'BgTaskLogListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});