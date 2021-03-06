REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.settings.DashboardPage');
REQUIRE('chlk.activities.settings.TeacherPage');
REQUIRE('chlk.activities.settings.PreferencesPage');
REQUIRE('chlk.activities.settings.StudentPage');
REQUIRE('chlk.activities.settings.AdminPage');
REQUIRE('chlk.activities.settings.AppSettingsPage');
REQUIRE('chlk.activities.settings.AdminPanoramaPage');
REQUIRE('chlk.activities.settings.AddCourseToPanoramaDialog');
REQUIRE('chlk.activities.settings.ReportCardsSettingsPage');
REQUIRE('chlk.activities.settings.AddSchoolToReportCardsDialog');

REQUIRE('chlk.models.settings.Dashboard');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.settings.AdminMessaging');
REQUIRE('chlk.models.settings.ReportCardsSettingsViewData');



REQUIRE('chlk.services.PreferenceService');
REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AdminDistrictService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.ReportingService');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SettingsController */
    CLASS(
        'SettingsController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.PreferenceService, 'preferenceService',

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [ria.mvc.Inject],
            chlk.services.SchoolService, 'schoolService',

            [ria.mvc.Inject],
            chlk.services.AdminDistrictService, 'adminDistrictService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.ReportingService, 'reportingService',

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
                dashboard.setCustomReportTemplatesVisible(true);
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
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardAdminAction() {
                var hasPermission = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS);
                var res = this.adminDistrictService.getSettings()
                    .then(function(data){
                        var msgViewData = new chlk.models.settings.MessagingSettingsViewData(data.getMessagingSettings(), data.getApplications(), hasPermission);
                        return msgViewData
                    })
                    .attach(this.validateResponse_());

                return this.PushView(chlk.activities.settings.AdminPage, res);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            function reportCardsAction(){
                var districtId = new chlk.models.id.DistrictId(window.districtId);
                var hasPermission = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS);
                var res = ria.async.wait([
                        this.reportingService.listReportCardsLogo(),
                        this.adminDistrictService.getSettings()])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var res = new chlk.models.settings.ReportCardsSettingsViewData(data[0], data[1].getApplications(), hasPermission);
                        this.getContext().getSession().set('reportCardsSettings', res);
                        return res;
                    }, this);
                return this.PushView(chlk.activities.settings.ReportCardsSettingsPage, res);
            },

            function showSchoolsDialogAction(){
                var reportCardsSettings = this.getContext().getSession().get('reportCardsSettings', null),
                    schoolIds = reportCardsSettings.getListOfLogo().filter(function(logo){return !logo.isDistrictLogo();}).map(function(school){return school.getSchoolId().valueOf()});
                this.WidgetStart('settings', 'showSchoolsDialog', [schoolIds])
                    .then(function(data){
                        var ids = data.schoolIds && data.schoolIds.split(',');
                        if(ids){
                            var names = data.schoolNames.split(',');
                            var schools = ids.map(function(id, index){
                                return new chlk.models.settings.ReportCardsLogo(Number(id), names[index]);
                            });

                            var schoolsModel = reportCardsSettings.getListOfLogo().concat(schools);
                            reportCardsSettings.setListOfLogo(schoolsModel);
                            this.getContext().getSession().set('reportCardsSettings', reportCardsSettings);
                            this.BackgroundUpdateView(chlk.activities.settings.ReportCardsSettingsPage, reportCardsSettings);
                        }
                    }, this);

                return null;
            },

            [[String, Array]],
            function showSchoolsDialogWidgetAction(requestId, excludedIds_){
                var res = this.schoolService.getLocalSchools()
                    .then(function(schools){
                        return new chlk.models.settings.AddSchoolToReportCardsViewData(schools, excludedIds_, requestId);
                    });

                return this.ShadeView(chlk.activities.settings.AddSchoolToReportCardsDialog, res);
            },

            [[Object]],
            function addSchoolToReportCardsAction(data){
                this.WidgetComplete(data.requestId, data);
                return this.CloseView(chlk.activities.settings.AddSchoolToReportCardsDialog);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [[Number, Object]],
            function updateReportCardsLogoAction(schoolId_, files_){
                var res = this.reportingService.updateReportCardsLogo(files_, schoolId_)
                    .attach(this.validateResponse_())
                    .then(function(listOfLogo){
                        var res = this.getContext().getSession().get('reportCardsSettings', null);

                        var newSchoolInfo = listOfLogo.filter(function(school){return schoolId_ && schoolId_ == school.getSchoolId() || !schoolId_ && school.isDistrictLogo()})[0],
                            oldListOfLogo = res.getListOfLogo(), oldIndex;

                        oldListOfLogo.forEach(function(school, index){
                            if(schoolId_ && schoolId_ == school.getSchoolId() || !schoolId_ && school.isDistrictLogo())
                                oldIndex = index;
                        });

                        if(!oldIndex && oldIndex !== 0)
                            oldIndex = oldListOfLogo.length;

                        oldListOfLogo[oldIndex] = newSchoolInfo;
                        res.setListOfLogo(oldListOfLogo);

                        this.getContext().getSession().set('reportCardsSettings', res);
                        return res;
                    }, this);
                return this.UpdateView(chlk.activities.settings.ReportCardsSettingsPage, res);
            },

            function prepareLogoItemsAfterDelete(schoolId_){
                var res = this.getContext().getSession().get('reportCardsSettings', null);

                var oldListOfLogo = res.getListOfLogo(), oldIndex;

                oldListOfLogo.forEach(function(school, index){
                    if(schoolId_ && schoolId_ == school.getSchoolId() || school.isDistrictLogo())
                        oldIndex = index;
                });

                oldListOfLogo.splice(oldIndex, 1);
                res.setListOfLogo(oldListOfLogo);

                this.getContext().getSession().set('reportCardsSettings', res);
                return res;
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [[chlk.models.id.ReportCardsLogoId, Number]],
            function deleteReportCardsLogoAction(reportCardsLogoId, schoolId_){
                var res;

                if(reportCardsLogoId)
                    res = this.reportingService.deleteReportCardsLogo(reportCardsLogoId)
                        .attach(this.validateResponse_())
                        .then(function(listOfLogo){
                            return this.prepareLogoItemsAfterDelete(schoolId_);
                        }, this);
                else
                    res = ria.async.DeferredData(this.prepareLogoItemsAfterDelete(schoolId_), 100);

                return this.UpdateView(chlk.activities.settings.ReportCardsSettingsPage, res);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function panoramaAdminAction() {
                var res = ria.async.wait([
                    this.adminDistrictService.getPanoramaSettings(),
                    this.adminDistrictService.getSettings(),
                    this.adminDistrictService.getStandardizedTests(),
                    this.classService.getCourseTypes()
                ])
                    .then(function(result){
                        var model = new chlk.models.settings.AdminPanoramaViewData(result[0], result[1].getApplications(), result[2]);
                        this.getContext().getSession().set(ChlkSessionConstants.ADMIN_PANORAMA, model);
                        this.getContext().getSession().set(ChlkSessionConstants.COURSE_TYPES, result[3]);
                        return model;
                    }, this)
                    .attach(this.validateResponse_());

                return this.PushOrUpdateView(chlk.activities.settings.AdminPanoramaPage, res);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function panoramaSubmitAction(data){
                if(data.submitType == 'subject'){
                    this.WidgetStart('settings', 'showCoursesDialog', [JSON.parse(data.excludedIds)])
                        .then(function(data){
                            var ids = data.courseTypeIds ? data.courseTypeIds.split(',').map(function(item){return parseInt(item, 10)}) : [],
                                model = this.getContext().getSession().get(ChlkSessionConstants.ADMIN_PANORAMA, null),
                                courseTypes = this.getContext().getSession().get(ChlkSessionConstants.COURSE_TYPES, null),
                                courseTypeDefaultSettings = [];

                            courseTypes = courseTypes.filter(function(item){return ids.indexOf(item.getCourseTypeId().valueOf()) > -1});
                            courseTypes.forEach(function(courseType){
                                courseTypeDefaultSettings.push(new chlk.models.panorama.CourseTypeSettingViewData(courseType));
                            });
                            model.getPanoramaSettings().setCourseTypeDefaultSettings(courseTypeDefaultSettings);
                            this.BackgroundUpdateView(chlk.activities.settings.AdminPanoramaPage, model, 'course-types');
                        }, this);

                    return null;
                }

                var res = this.adminDistrictService.savePanoramaSettings(JSON.parse(data.filters))
                    .thenCall(this.adminDistrictService.getPanoramaSettings, [])
                    .then(function(panoramaSettings){
                        var model = this.getContext().getSession().get(ChlkSessionConstants.ADMIN_PANORAMA, null);
                        model.setPanoramaSettings(panoramaSettings);
                        return model;
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.settings.AdminPanoramaPage, res);
            },

            [[String, Array]],
            function showCoursesDialogWidgetAction(requestId, excludedIds_){
                var courseTypes = this.getContext().getSession().get(ChlkSessionConstants.COURSE_TYPES, null);
                var model = new chlk.models.settings.AddCourseToPanoramaViewData(courseTypes, excludedIds_, requestId);
                return this.ShadeView(chlk.activities.settings.AddCourseToPanoramaDialog, ria.async.DeferredData(model));
            },

            function addCoursesToPanoramaAction(data){
                this.WidgetComplete(data.requestId, data);
                return this.CloseView(chlk.activities.settings.AddCourseToPanoramaDialog);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.id.AppId]],
            function appSettingsAction(appId) {

                if(!this.isStudyCenterEnabled() && !this.isAssessmentEnabled())
                    return this.ShowMsgBox('Current school doesn\'t support assessments, applications, study center, profile explorer', null, null, 'error'), null;

                var mode = "settingsview";

                var result = ria.async.wait([
                    this.adminDistrictService.getSettings(),
                    this.appsService.getAccessToken(this.getCurrentPerson().getId(), null, appId)
                ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var applications = result[0].getApplications(),
                            data = result[1],
                            appData = data.getApplication();

                        var viewUrl = appData.getUrl() + '?mode=' + mode.valueOf()
                            + '&apiRoot=' + encodeURIComponent(_GLOBAL.location.origin)
                            + '&token=' + encodeURIComponent(data.getToken());

                        return new chlk.models.settings.AppSettingsViewData(null, appData, viewUrl, '', applications);
                    }, this);

                return this.PushView(chlk.activities.settings.AppSettingsPage, result);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS]
            ])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.settings.AdminMessaging]],
            function updateMessagingSettingsAction(model) {
                this.getContext().getSession().set(ChlkSessionConstants.MESSAGING_SETTINGS, model);
                this.schoolService.updateMessagingSettings(null, model.isAllowedForStudents(), model.isAllowedForStudentsInTheSameClass(),
                    model.isAllowedForTeachersToStudents(), model.isAllowedForTeachersToStudentsInTheSameClass());
                return null;
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
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.APPTESTER
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardAppTesterAction() {
                var dashboard = new chlk.models.settings.Dashboard();
                dashboard.setDbMaintenanceVisible(false);
                dashboard.setBackgroundTaskMonitorVisible(false);
                dashboard.setStorageMonitorVisible(false);
                dashboard.setPreferencesVisible(false);
                dashboard.setAppCategoriesVisible(false);
                dashboard.setDepartmentsVisible(false);
                dashboard.setUpgradeSchoolsVisible(false);
                dashboard.setCustomReportTemplatesVisible(false);
                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.ASSESSMENTADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            function dashboardAssessmentAdminAction() {
                var dashboard = new chlk.models.settings.Dashboard();
                dashboard.setDbMaintenanceVisible(false);
                dashboard.setBackgroundTaskMonitorVisible(false);
                dashboard.setStorageMonitorVisible(false);
                dashboard.setPreferencesVisible(false);
                dashboard.setAppCategoriesVisible(false);
                dashboard.setDepartmentsVisible(false);
                dashboard.setUpgradeSchoolsVisible(false);
                dashboard.setCustomReportTemplatesVisible(false);
                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            }
        ])
});
