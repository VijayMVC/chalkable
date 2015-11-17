REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.DistrictService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.school.SchoolDetailsPage');
REQUIRE('chlk.activities.school.SchoolPeoplePage');
REQUIRE('chlk.activities.school.ActionButtonsPopup');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.activities.school.SchoolSisPage');
REQUIRE('chlk.activities.school.SchoolsListPage');
REQUIRE('chlk.activities.school.ImportSchoolDialog');
REQUIRE('chlk.activities.school.UpgradeDistrictsPage');
REQUIRE('chlk.activities.school.SchoolClassesSummaryPage');

REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.schoolImport.ImportTaskData');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.school.UpgradeDistrictsViewData');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SchoolsController */
    CLASS(
        'SchoolsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.AccountService, 'accountService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.DistrictService, 'districtService',

        [chlk.controllers.SidebarButton('schools')],
        [[chlk.models.id.DistrictId, Number]],
        function pageAction(districtId_, start_) {
            return this.UpdateView(chlk.activities.school.SchoolsListPage, this.getSchools_(districtId_, start_ || 0));
        },

        [chlk.controllers.SidebarButton('schools')],
        [[chlk.models.id.DistrictId, Number]],
        function listAction(districtId, pageIndex_) {
            return this.PushView(chlk.activities.school.SchoolsListPage, this.getSchools_(districtId, pageIndex_ | 0));
        },

        [[chlk.models.id.DistrictId, Number, Number]],
        ria.async.Future, function getSchools_(distrcitId, start_, count_){
            return this.schoolService
                .getSchools(distrcitId, start_, count_)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new ria.async.DeferredData(new chlk.models.school.SchoolListViewData(distrcitId, data));
                }, this);
        },


        [[chlk.models.id.DistrictId]],
        VOID, function importAction(districtId) {
            var result = this.schoolService
                .getSchoolsForImport(districtId)
                .attach(this.validateResponse_())
                .then(function(data){
                    var schools = data.getItems();
                    return new ria.async.DeferredData(new chlk.models.schoolImport.SchoolImportViewData(districtId, schools));
                });
                return this.ShadeView(chlk.activities.school.ImportSchoolDialog, result);
        },

        [[chlk.models.schoolImport.ImportTaskData]],
        VOID, function importSchoolsAction(model) {
            var result = this.schoolService
                .runSchoolImport(
                    model.getDistrictId(),
                    model.getSisSchoolId(),
                    model.getSisSchoolYearId()
                )
                .attach(this.validateResponse_())
                .then(function(data){
                    if (data)
                        alert('Task was scheduled');
                });
        },


        [[chlk.models.id.SchoolId]],
        function detailsAction(id) {
            var result = this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
        },
        [[chlk.models.id.SchoolId]],
        function sisInfoAction(id) {
            var result = this.schoolService
                .getSisInfo(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolSisPage, result);
        },


        ArrayOf(chlk.models.common.NameId),  function convertRolesToNameIds_(roles){
            var res = [];
            for(var i =0; i < roles.length; i++){
                res.push(new chlk.models.common.NameId(roles[i].getRoleId().valueOf(), roles[i].getRole));
            }
            return res;
        },
        ArrayOf(chlk.models.common.NameId), function convertGradeLevelsToNameIds_(gradeLevels){
            var res = [];
            for(var i = 0; i < gradeLevels.length; i++){
                res.push(new chlk.models.common.NameId(gradeLevels[i].getId().valueOf(), gradeLevels[i].getName()));
            }
            return res;
        },

        //TODO:refactor
        [[chlk.models.id.SchoolId]],
        function peopleAction(id) {
            var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
            var newGradeLevels = this.convertGradeLevelsToNameIds_(this.gradeLevelService.getGradeLevels());
            newGradeLevels.unshift(serializer.deserialize({name: 'All Grades', id: null}, chlk.models.common.NameId));
            var roles =  this.convertRolesToNameIds_(this.accountService.getSchoolRoles());
            roles.push(serializer.deserialize({name: 'All Roles', id: null}, chlk.models.common.NameId));

            var result = ria.async.wait([
                    this.schoolService.getPeopleSummary(id),
                    this.schoolService.getUsers(id, null, null,0, null, false)
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var users = new chlk.models.people.UsersList(result[1], true);
                    return new chlk.models.school.SchoolPeople(users, roles, newGradeLevels, result[0]);
                });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        },



        [[chlk.models.people.UsersList]],
        VOID, function setPeopleFilterAction(model_) {
//            var result = ria.async.wait([
//                this.schoolService.getUsers(
//                    model_.getSchoolId(),
//                    model_.getRoleId() || null,
//                    model_.getGradeLevelId() || null,
//                    model_.isByLastName(),
//                    0
//                )
//            ]).then(function(result){
//                model_.setUsers(result[0]);
//                model_.setSelectedIndex(0);
//                return model_;
//            });
//            return this.UpdateView(chlk.activities.school.SchoolPeoplePage, result);
        },



        //TODO:refactor
        [[chlk.models.id.SchoolId]],
        VOID, function actionButtonsAction(id) {
            var result =  this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_())
                .then(function(data){
                        return new chlk.models.school.ActionButtons(
                            data.getButtons(),
                            data.getEmails(),
                            chlk.controls.getActionLinkControlLastNode()
                        );
                });
            return this.ShadeView(chlk.activities.school.ActionButtonsPopup, result);
        },

        [[Object]],
        function actionLinkAction(form_){
            if(confirm(form_.index + ' ' + form_.email))
                this.context.getDefaultView().getCurrent().close();
        },

        function tryToUpgradeSchoolsSysAdminAction(){
            var res = this.districtService.getDistricts(0, Number.MAX_SAFE_INTEGER)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.school.UpgradeDistrictsViewData(data, null);
                }, this);
            return this.PushView(chlk.activities.school.UpgradeDistrictsPage, res);
        },

        [[chlk.models.id.DistrictId]],
        function showSchoolsForUpgradeAction(districtId){
            var res = this.getSchools_(districtId, 0, 10000);
            return this.UpdateView(chlk.activities.school.UpgradeDistrictsPage, res);
        },

        [[chlk.models.id.DistrictId]],
        function upgradeDistrictSysAdminAction(distrcitId){
            return this.upgradeSchools_(distrcitId, null);
        },
        [[chlk.models.id.SchoolId]],
        function upgradeSchoolSysAdminAction(schoolId){
            return this.upgradeSchools_(null, schoolId);
        },
        [[chlk.models.id.DistrictId]],
        function downgradeDistrictSysAdminAction(distrcitId){
            return this.downgradeSchools_(distrcitId, null);
        },
        [[chlk.models.id.SchoolId]],
        function downgradeSchoolSysAdminAction(schoolId){
            return this.downgradeSchools_(null, schoolId);
        },


        [[chlk.models.id.DistrictId, chlk.models.id.SchoolId]],
        function upgradeSchools_(distrcitId, schoolId){
            this.ShowPromptBox('Please choose upgrade till date', ''
                , (new chlk.models.common.ChlkDate()).toStandardFormat()
                , null, 'datepicker')
                .then(function(selectedDate){
                    var date = new chlk.models.common.ChlkDate(new Date(selectedDate));
                    return this.upgradeDownGradeSchools_(distrcitId, schoolId, date);
                }, this);
        },

        [[chlk.models.id.DistrictId, chlk.models.id.SchoolId]],
        function downgradeSchools_(distrcitId, schoolId){
            this.ShowConfirmBox('Do you really want to downgrade current school or district?', '')
                .then(function(data){
                   return this.upgradeDownGradeSchools_(distrcitId, schoolId, null);
                }, this);
        },

        [[chlk.models.id.DistrictId, chlk.models.id.SchoolId, chlk.models.common.ChlkDate]],
        function upgradeDownGradeSchools_(districtId, schoolId, tillDate){
            return this.schoolService.updateStudyCenter(districtId, schoolId, tillDate)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('schools', 'tryToUpgradeSchools', []);
                }, this);
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.id.SchoolId, String]],
        function classesSummaryAction(schoolId, schoolName){
            var result = this.classService.getClassesStatistic(schoolId)
                .then(function(classes){
                    var clazz = classes.getItems()[0];
                    if(clazz)
                        clazz.setSchoolId(schoolId);
                    return new chlk.models.school.SchoolClassesSummaryViewData(schoolName, schoolId, classes);
                })
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolClassesSummaryPage, result);
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.school.SchoolClassesSummaryViewData]],
        function classesStatisticFilterAction(model){
            return this.classesStatisticAction(0, model.getFilter(), model.getSchoolId());
        },

        [chlk.controllers.SidebarButton('classes')],
        [[Number, String, chlk.models.id.SchoolId]],
        function classesStatisticAction(pageIndex, filter_, schoolId_){
            var start = 10 * pageIndex;
            var result = this.classService.getClassesStatistic(schoolId_, start, filter_)
                .then(function(model){
                    filter_ && model.setFilter(filter_);
                    var clazz = model.getItems()[0];
                    if(clazz)
                        clazz.setSchoolId(schoolId_);
                    return model;
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.school.SchoolClassesSummaryPage, result);
        }

    ])
});
