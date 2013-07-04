REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.settings.DashboardPage');
REQUIRE('chlk.models.settings.Dashboard');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SettingsController */
    CLASS(
        'SettingsController', EXTENDS(chlk.controllers.BaseController), [

            [chlk.controllers.SidebarButton('settings')],
            function dashboardAction() {
                //if it's sysadmin check
                var dashboard = new chlk.models.settings.Dashboard();
                dashboard.setBackgroundTaskMonitorVisible(true);
                dashboard.setStorageMonitorVisible(true);
                dashboard.setPreferencesVisible(true);
                dashboard.setAppCategoriesVisible(true);
                dashboard.setDepartmentsVisible(true);

                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            }
        ])
});
