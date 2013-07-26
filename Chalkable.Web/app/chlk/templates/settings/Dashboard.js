REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.settings.Dashboard');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.Dashboard*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/Dashboard.jade')],
        [ria.templates.ModelBind(chlk.models.settings.Dashboard)],
        'Dashboard', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'departmentsVisible',
            [ria.templates.ModelPropertyBind],
            Boolean, 'appCategoriesVisible',
            [ria.templates.ModelPropertyBind],
            Boolean, 'storageMonitorVisible',
            [ria.templates.ModelPropertyBind],
            Boolean, 'preferencesVisible',
            [ria.templates.ModelPropertyBind],
            Boolean, 'backgroundTaskMonitorVisible',
            [ria.templates.ModelBind],
            Boolean, 'dbMaintenanceVisible'
        ])
});