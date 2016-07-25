REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.DistrictService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.SchoolYearService');
REQUIRE('chlk.services.TeacherService');

REQUIRE('chlk.activities.school.SchoolDetailsPage');
REQUIRE('chlk.activities.school.SchoolPeoplePage');
REQUIRE('chlk.activities.school.ActionButtonsPopup');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.activities.school.SchoolSisPage');
REQUIRE('chlk.activities.school.SchoolsListPage');
REQUIRE('chlk.activities.school.ImportSchoolDialog');
REQUIRE('chlk.activities.school.UpgradeDistrictsPage');
REQUIRE('chlk.activities.school.SchoolClassesSummaryPage');
REQUIRE('chlk.activities.school.SchoolTeachersSummaryPage');

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
        chlk.services.TeacherService, 'teacherService',

        [ria.mvc.Inject],
        chlk.services.SchoolYearService, 'schoolYearService',

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
            this.schoolService.updateStudyCenter(districtId, schoolId, tillDate)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('schools', 'tryToUpgradeSchools', []);
                }, this);
            return null;
        },

        [[chlk.models.id.DistrictId, chlk.models.id.SchoolId, Boolean]],
        function enableAssessmentAction(districtId, schoolId_){
            return this.updateAssessmentEnabled_(districtId, schoolId_, true);
        },

        [[chlk.models.id.DistrictId, chlk.models.id.SchoolId, Boolean]],
        function disableAssessmentAction(districtId, schoolId_){
            return this.updateAssessmentEnabled_(districtId, schoolId_, false);
        },

        [[chlk.models.id.DistrictId, chlk.models.id.SchoolId, Boolean, Boolean]],
        function updateAssessmentEnabled_(districtId, schoolId_, enabled){
            this.schoolService.updateAssessmentEnabled(districtId, schoolId_, enabled)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('schools', 'tryToUpgradeSchools', []);
                }, this);
            return null;
        },

        function getCurrentYearId_(years){
            if(!years)
                return null;

            var dt = getDate(), currentSchoolYearId;
            years.forEach(function(year){
                if(year.getStartDate() && year.getStartDate().getDate() <= dt)
                    currentSchoolYearId = year.getId();
            });
            return currentSchoolYearId;
        },

        function prepareClassForSummary(classes){
            classes.forEach(function(clazz){
                clazz.setAttendancesProfileEnabled(!this.isPageReadonly_('VIEW_CLASSROOM_ATTENDANCE', 'VIEW_CLASSROOM_ATTENDANCE_ADMIN', clazz));
                clazz.setDisciplinesProfileEnabled(!this.isPageReadonly_('VIEW_CLASSROOM_DISCIPLINE', 'VIEW_CLASSROOM_DISCIPLINE_ADMIN', clazz));
                clazz.setGradingProfileEnabled(!this.isPageReadonly_('VIEW_CLASSROOM', 'VIEW_CLASSROOM_ADMIN', clazz));
            }, this);
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.id.SchoolId, String, chlk.models.id.SchoolYearId, chlk.models.id.SchoolPersonId, Number, String]],
        function classesSummaryAction(schoolId, schoolName, schoolYearId_, teacherId_, sortType_, filter_){
            sortType_ = sortType_ || chlk.models.admin.ClassSortTypeEnum.CLASS_ASC.valueOf();
            var result = this.schoolYearService.list(schoolId)
                .then(function(years){
                    var currentSchoolYearId = schoolYearId_ || this.getCurrentYearId_(years);
                    return this.classService.getClassesStatistic(currentSchoolYearId, 0, filter_, teacherId_, sortType_)
                        .then(function(classes){
                            this.prepareClassForSummary(classes);

                            return new chlk.models.school.SchoolSummaryViewData(schoolName, schoolId, currentSchoolYearId, years,
                                new chlk.models.admin.BaseStatisticGridViewData(classes, sortType_, schoolId, schoolName, filter_, teacherId_, currentSchoolYearId),
                                filter_, teacherId_);
                        }, this)
                }, this)
                .catchException(chlk.lib.exception.ChalkableException, function(exception) {
                    return this.ShowMsgBox(exception.getMessage(), 'oops',[{ text: Msg.GOT_IT.toUpperCase() }])
                        .then(function(){
                            this.BackgroundCloseView(chlk.activities.lib.PendingActionDialog);
                            this.redirectToPage_('district', 'summary', [])
                        }, this)
                        .thenBreak();
                }, this)
                .attach(this.validateResponse_());

            return this.PushView(chlk.activities.school.SchoolClassesSummaryPage, result);
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.id.SchoolId, String,chlk.models.id.SchoolYearId, chlk.models.id.SchoolPersonId, Number, String]],
        function classesSummaryTeacherAction(schoolId_, schoolName_, schoolYearId_, teacherId_, sortType_, filter_){
            sortType_ = sortType_ || chlk.models.admin.ClassSortTypeEnum.CLASS_ASC.valueOf();
            teacherId_ = teacherId_ || this.getCurrentPerson().getId();
            schoolName_ = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_NAME, null);
            var currentSchoolYearId = schoolYearId_ || this.getCurrentSchoolYearId();
            var result = this.classService.getClassesStatistic(currentSchoolYearId, 0, filter_, teacherId_, sortType_)
                .then(function(classes){
                    this.prepareClassForSummary(classes);

                    return new chlk.models.school.SchoolSummaryViewData(schoolName_, null, currentSchoolYearId, null,
                        new chlk.models.admin.BaseStatisticGridViewData(classes, sortType_, null, schoolName_, filter_, teacherId_, currentSchoolYearId),
                        filter_, teacherId_);
                }, this)
                .attach(this.validateResponse_());

            return this.PushView(chlk.activities.school.SchoolClassesSummaryPage, result);
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.admin.BaseStatisticGridViewData]],
        function classesStatisticAction(model){
            var isFilter = model.getSubmitType() != 'scroll';
            var start = isFilter ? 0 : model.getStart();
            var result = this.classService.getClassesStatistic(model.getSchoolYearId(), start, model.getFilter(), model.getTeacherId(), model.getSortType())
                .then(function(resModel){
                    this.prepareClassForSummary(resModel);
                    return new chlk.models.admin.BaseStatisticGridViewData(resModel, model.getSortType(), model.getSchoolId(), model.getSchoolName(), model.getFilter(), model.getTeacherId(), model.getSchoolYearId());
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.school.SchoolClassesSummaryPage, result, isFilter ? null : chlk.activities.lib.DontShowLoader());
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.id.SchoolId, String, Number, chlk.models.id.SchoolYearId, String]],
        function teachersSummaryAction(schoolId, schoolName, sortType_, schoolYearId_, filter_){
            sortType_ = sortType_ || chlk.models.admin.TeacherSortTypeEnum.TEACHER_ASC.valueOf();
            var result = this.schoolYearService.list(schoolId)
                .then(function(years){
                    var currentSchoolYearId = schoolYearId_ || this.getCurrentYearId_(years);
                    return this.teacherService.getTeachersStats(currentSchoolYearId, null, filter_, sortType_)
                        .then(function(teachers){
                            var teacher = teachers[0];
                            if(teacher){
                                teacher.setSchoolYearId(currentSchoolYearId);
                                teacher.setSchoolId(schoolId);
                                teacher.setSchoolName(schoolName);
                            }

                            return new chlk.models.school.SchoolSummaryViewData(schoolName, schoolId, currentSchoolYearId, years,
                                new chlk.models.admin.BaseStatisticGridViewData(teachers, sortType_, schoolId, schoolName, filter_, null, currentSchoolYearId),
                                filter_);
                        })
                }, this)
                .attach(this.validateResponse_());

            return this.PushView(chlk.activities.school.SchoolTeachersSummaryPage, result);
        },

        [chlk.controllers.SidebarButton('classes')],
        [[chlk.models.admin.BaseStatisticGridViewData]],
        function teachersStatisticAction(model){
            var isFilter = model.getSubmitType() != 'scroll';
            var start = isFilter ? 0 : model.getStart();
            var result = this.teacherService.getTeachersStats(model.getSchoolYearId(), start, model.getFilter(), model.getSortType())
                .then(function(items){
                    var teacher = items[0];
                    if(teacher){
                        teacher.setSchoolYearId(model.getSchoolYearId());
                        teacher.setSchoolId(model.getSchoolId());
                        teacher.setSchoolName(model.getSchoolName());
                    }

                    return new chlk.models.admin.BaseStatisticGridViewData(items, model.getSortType(), model.getSchoolId(), model.getSchoolName(), model.getFilter(), null, model.getSchoolYearId());
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.school.SchoolTeachersSummaryPage, result, isFilter ? null : chlk.activities.lib.DontShowLoader());
        }

    ])
});
