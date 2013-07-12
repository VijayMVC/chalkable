REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.settings.Dashboard');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.DashboardPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.settings.Dashboard)],
        'DashboardPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});