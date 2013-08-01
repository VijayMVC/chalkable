REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.settings.DashboardPage');
REQUIRE('chlk.activities.settings.TeacherPage');
REQUIRE('chlk.activities.settings.PreferencesPage');
REQUIRE('chlk.activities.settings.DeveloperPage');
REQUIRE('chlk.models.settings.Dashboard');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.services.SettingsService');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SettingsController */
    CLASS(
        'SettingsController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.SettingsService, 'settingsService',

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],

            [chlk.controllers.SidebarButton('settings')],
            function dashboardAction() {
                var dashboard = new chlk.models.settings.Dashboard();
                dashboard.setDbMaintenanceVisible(true);
                dashboard.setBackgroundTaskMonitorVisible(true);
                dashboard.setStorageMonitorVisible(true);
                dashboard.setPreferencesVisible(true);
                dashboard.setAppCategoriesVisible(true);
                dashboard.setDepartmentsVisible(true);

                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function preferencesAction() {
                 var result = this.settingsService.getPreferences();
                 return this.PushView(chlk.activities.settings.PreferencesPage, result);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.settings.Preference]],
            function setPreferenceAction(model) {
                var result = this.settingsService.setPreference(
                    model.getKey(),
                    model.getValue(),
                    model.isPublicPreference()
                );
                return this.UpdateView(chlk.activities.settings.PreferencesPage, result);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.TEACHER
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardTeacherAction() {

                var teacherSettings = new chlk.models.settings.TeacherSettings();
                return this.PushView(chlk.activities.settings.TeacherPage, ria.async.DeferredData(teacherSettings));
            },
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardDeveloperAction() {

                var devSettings = new chlk.models.settings.DeveloperSettings();
                devSettings.setDeveloperId(this.getCurrentPerson().getId());
                return this.PushView(chlk.activities.settings.DeveloperPage, ria.async.DeferredData(devSettings));
            }

        ])
});
