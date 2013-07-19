REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.bgtasks.BgTasks');

NAMESPACE('chlk.activities.bgtasks', function () {

    /** @class chlk.activities.bgtasks.BgTasksListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.bgtasks.BgTasks)],
        'BgTasksListPage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});