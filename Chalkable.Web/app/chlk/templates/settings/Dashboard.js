REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.settings.Dashboard');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.Dashboard*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/Dashboard.jade')],
        [ria.templates.ModelBind(chlk.models.settings.Dashboard)],
        'Dashboard', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Boolean, 'departmentsVisible',
            [ria.templates.ModelBind],
            Boolean, 'appCategoriesVisible',
            [ria.templates.ModelBind],
            Boolean, 'storageMonitorVisible',
            [ria.templates.ModelBind],
            Boolean, 'preferencesVisible',
            [ria.templates.ModelBind],
            Boolean, 'backgroundTaskMonitorVisible',
            [ria.templates.ModelBind],
            Boolean, 'dbMaintenanceVisible'
        ])
});