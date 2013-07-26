REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.settings.DashboardPage');
REQUIRE('chlk.activities.settings.TeacherPage');
REQUIRE('chlk.models.settings.Dashboard');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SettingsController */
    CLASS(
        'SettingsController', EXTENDS(chlk.controllers.BaseController), [

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardAction() {
                //if it's sysadmin check
                var dashboard = new chlk.models.settings.Dashboard();
                dashboard.setBackgroundTaskMonitorVisible(true);
                dashboard.setStorageMonitorVisible(true);
                dashboard.setPreferencesVisible(true);
                dashboard.setAppCategoriesVisible(true);
                dashboard.setDepartmentsVisible(true);
                dashboard.setDbMaintenanceVisible(true);
                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            },



            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.TEACHER
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardTeacherAction() {

                var teacherSettings = new chlk.models.settings.TeacherSettings();
                return this.PushView(chlk.activities.settings.TeacherPage, ria.async.DeferredData(teacherSettings));
            }

        ])
});
