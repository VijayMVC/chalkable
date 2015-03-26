REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.settings.DashboardPage');
REQUIRE('chlk.activities.settings.TeacherPage');
REQUIRE('chlk.activities.settings.PreferencesPage');
REQUIRE('chlk.activities.settings.StudentPage');
REQUIRE('chlk.models.settings.Dashboard');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.services.PreferenceService');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SettingsController */
    CLASS(
        'SettingsController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.PreferenceService, 'preferenceService',

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

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
                dashboard.setUpgradeSchoolsVisible(true);
                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.settings.PreferenceCategoryEnum]],
            function preferencesAction(category_) {
                var category_ = category_ || chlk.models.settings.PreferenceCategoryEnum.COMMON;
                var result = this.preferenceService
                    .getPreferences(category_)
                    .attach(this.validateResponse_())
                    .then(function(preferences){
                        return this.preparePreferencesList_(category_, preferences);
                    }, this);
                return this.PushOrUpdateView(chlk.activities.settings.PreferencesPage, result);
            },

            [[chlk.models.settings.PreferenceCategoryEnum, ArrayOf(chlk.models.settings.Preference)]],
            chlk.models.settings.PreferencesList, function preparePreferencesList_(categoryId, preferences){
                var category = null;
                if(categoryId){
                    category = new chlk.models.settings.PreferenceCategory(categoryId);
                }
                var categories = this.preferenceService.getPreferencesCategories();
                return new chlk.models.settings.PreferencesList(preferences, categories, category);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.settings.Preference]],
            function setPreferenceAction(model) {
                var result = this.preferenceService.setPreference(
                    model.getKey(),
                    model.getValue(),
                    model.isPublicPreference()
                )
                .then(function(preferences){
                    preferences = preferences.filter(function(item){return item.getCategory() == model.getCategory();});
                    return this.preparePreferencesList_(model.getCategory(), preferences);
                }, this);
                return this.UpdateView(chlk.activities.settings.PreferencesPage, result);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.TEACHER
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardTeacherAction() {
                var teacherSettings = new chlk.models.settings.TeacherSettings(this.getCurrentPerson().getId());
                return this.PushView(chlk.activities.settings.TeacherPage, ria.async.DeferredData(teacherSettings));
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.STUDENT
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardStudentAction() {
                var studentSettings = new chlk.models.settings.SchoolPersonSettings(this.getCurrentPerson().getId());
                return this.PushView(chlk.activities.settings.StudentPage, ria.async.DeferredData(studentSettings));
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardDeveloperAction() {
               return this.Redirect('account', 'profile')
            }
        ])
});
