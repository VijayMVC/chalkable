REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.bgtasks.BgTasks');

NAMESPACE('chlk.activities.bgtasks', function () {

    /** @class chlk.activities.bgtasks.BgTasksListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.bgtasks.BgTasks)],
        'BgTasksListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});