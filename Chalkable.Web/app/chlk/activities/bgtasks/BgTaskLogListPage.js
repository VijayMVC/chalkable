REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.bgtasks.BgTaskLogs');

NAMESPACE('chlk.activities.bgtasks', function () {

    /** @class chlk.activities.bgtasks.BgTaskLogListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.bgtasks.BgTaskLogs)],
        'BgTaskLogListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});