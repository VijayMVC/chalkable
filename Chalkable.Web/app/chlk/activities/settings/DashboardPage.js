REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.Dashboard');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.DashboardPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.settings.Dashboard)],
        'DashboardPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});