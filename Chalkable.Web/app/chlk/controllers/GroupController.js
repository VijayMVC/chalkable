REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.GroupService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.TeacherService');

REQUIRE('chlk.models.recipients.RecipientsSubmitViewData');

REQUIRE('chlk.activities.announcement.GroupStudentsFilterDialog');
REQUIRE('chlk.activities.recipients.GroupSelectorDialog');
REQUIRE('chlk.activities.recipients.GroupCreateDialog');
REQUIRE('chlk.activities.recipients.PeoplePage');
REQUIRE('chlk.activities.recipients.AdminPeoplePage');

NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.GroupController */
    CLASS(
        [chlk.controllers.NotChangedSidebarButton],
        'GroupController', EXTENDS(chlk.controllers.BaseController), [
            [ria.mvc.Inject],
            chlk.services.GroupService, 'groupService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.StudentService, 'studentService',

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [ria.mvc.Inject],
            chlk.services.SchoolService, 'schoolService',

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId]],
            function showCreateEditDialogAction(groupId_){
                var activityClass = this.getView().getCurrent().getClass();
                this.WidgetStart('group', 'showCreateEditDialog', [groupId_])
                    .then(function(data){
                        this.BackgroundNavigate('group', groupId_ ? 'editGroup' : 'createGroup', [data, activityClass]);
                    }, this)
                    .attach(this.validateResponse_());
                return null;
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.recipients.RecipientsSubmitViewData]],
            function groupSubmitAction(model) {
                this.BackgroundCloseView(chlk.activities.recipients.GroupCreateDialog);
                this.WidgetComplete(model.getRequestId(), model);
                return null;
            },

            [chlk.controllers.NotChangedSidebarButton()],
            function createGroupAction(model, activityClass) {
                var ids = model.getStudentIds().split(',').map(function(item){return new chlk.models.id.SchoolPersonId(item)});
                var res = this.groupService.create(model.getName(), ids)
                    .then(function(group){
                        group.setNewAdded(true);
                        return new chlk.models.recipients.GroupsListViewData([group]);
                    }.bind(this));

                return this.UpdateView(activityClass, res, 'add-group');
            },

            [chlk.controllers.NotChangedSidebarButton()],
            function editGroupAction(model, activityClass) {
                var ids = model.getStudentIds().split(',').map(function(item){return new chlk.models.id.SchoolPersonId(item)});
                var res = this.groupService.edit(model.getGroupId(), model.getName(), ids);

                return this.UpdateView(activityClass, res, 'edit-group');
            },

            function getGroupPersonInfo_(type, requestId_, data_, groupId_, topData_){
                var withGroups = this.userIsAdmin() && (type == chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS ||
                        type == chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS),
                    isView = type == chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS, itemsCount = 30,
                    classId = topData_ && topData_.getSelectedItemId(), isStudent = this.getCurrentRole().isStudent();

                return ria.async.wait([
                        withGroups ? this.groupService.list() : ria.async.DeferredData([]),
                        this.studentService.getStudents(classId, null, !isStudent, null, 0, itemsCount),
                        isStudent ? this.teacherService.getTeachers(classId, null, null, 0, itemsCount, true) :
                            this.studentService.getStudents(classId, null, false, null, 0, itemsCount),
                        (!isView || this.userIsAdmin() ) ? this.schoolService.getUserLocalSchools() : ria.async.DeferredData([]),
                        isStudent ? ria.async.DeferredData([]) : this.schoolService.getSchoolPrograms(),
                        groupId_ ? this.groupService.info(groupId_) : ria.async.DeferredData(null)//,
                        //classId_ ? this.classService.getClassesBySchool(classId_) : ria.async.DeferredData(null)
                    ])
                    .then(function(result){
                        var leParams = this.getContext().getSession().get(ChlkSessionConstants.LE_PARAMS, new chlk.models.school.LEParams()),
                            hasAccessToAllStudents = (isStudent || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_STUDENT)) &&
                                (!classId || !classId.valueOf()),
                            hasAccessToLE = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS) && leParams.isLEIntegrated(),
                            hasOwnStudents = result[1].getItems().length > 0,
                            groupsModel = new chlk.models.recipients.GroupsListViewData(result[0]),
                            gradeLevels = this.getContext().getSession().get(ChlkSessionConstants.GRADE_LEVELS),
                            schools = result[3],
                            programs = result[4],
                            myStudentsPart = new chlk.models.recipients.UsersListViewData(result[1], true, gradeLevels, schools, programs),
                            allStudentsPart = new chlk.models.recipients.UsersListViewData(result[2], false, gradeLevels, schools, programs),
                            selectedGroups = (data_ && data_.getSelected() && data_.getSelected().groups) || [],
                            selectedStudents = (groupId_ ? result[5].getStudents() : (data_ && data_.getSelected() && data_.getSelected().students)) || [];
                        var model = new chlk.models.recipients.GroupSelectorViewData(requestId_, type, hasAccessToAllStudents, hasOwnStudents,
                            groupsModel, myStudentsPart, allStudentsPart, selectedGroups, selectedStudents, result[5], hasAccessToLE, topData_);
                        return model;
                    }, this)
                    .attach(this.validateResponse_());
            },

            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function peopleAction(classId_) {
                var classesBarData = new chlk.models.classes.ClassesForTopBar(null, classId_);

                var res = this.getGroupPersonInfo_(chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS, null, null, null, classesBarData);

                return this.PushView(chlk.activities.recipients.PeoplePage, res);
            },

            [chlk.controllers.SidebarButton('people')],
            function peopleDistrictAdminAction() {
                var res = this.getGroupPersonInfo_(chlk.models.recipients.SelectorModeEnum.VIEW_WITH_GROUPS);

                return this.PushView(chlk.activities.recipients.AdminPeoplePage, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[String, chlk.models.id.GroupId]],
            function showCreateEditDialogWidgetAction(requestId, groupId_) {
                var res = this.getGroupPersonInfo_(groupId_ ? chlk.models.recipients.SelectorModeEnum.EDIT_WITHOUT_GROUPS : 
                    chlk.models.recipients.SelectorModeEnum.SELECT_WITHOUT_GROUPS, requestId, null, groupId_);

                return this.ShadeOrUpdateView(chlk.activities.recipients.GroupCreateDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[String, chlk.models.group.AnnouncementGroupsViewData]],
            function showWidgetAction(requestId, data) {
                var res = this.getGroupPersonInfo_(chlk.models.recipients.SelectorModeEnum.SELECT_WITH_GROUPS, requestId, data);

                return this.ShadeOrUpdateView(chlk.activities.recipients.GroupSelectorDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.recipients.RecipientsSubmitViewData]],
            function submitGroupsAction(model) {
                this.BackgroundCloseView(chlk.activities.recipients.GroupSelectorDialog);
                this.WidgetComplete(model.getRequestId(), model);
                return null;
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId]],
            function showGroupMembersAction(groupId) {
                var res = this.groupService
                    .groupExplorer(groupId)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId, String]],
            function showGradeLevelMembersAction(groupId, schoolYearId, gradeLevelId, classIds_) {
                var res = this.groupService
                    .studentForGroup(groupId, schoolYearId, gradeLevelId, classIds_ && this.getIdsList(classIds_, chlk.models.id.ClassId))
                    .then(function (students) {
                        return new chlk.models.group.StudentsForGroupViewData(groupId, gradeLevelId, schoolYearId, students);
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res, classIds_ ? 'after-filter' : '');
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
            function assignGradeLevelAction(groupId, schoolYearId, gradeLevelId) {
                var res = this.groupService
                    .assignGradeLevel(groupId, schoolYearId, gradeLevelId)
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
            function unAssignGradeLevelAction(groupId, schoolYearId, gradeLevelId) {
                var res = this.groupService
                    .unassignGradeLevel(groupId, schoolYearId, gradeLevelId)
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolPersonId]],
            function assignStudentAction(groupId, studentId) {
                if (!studentId)
                    return null;
                var res = this.groupService
                    .assignStudents(groupId, [studentId])
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolPersonId]],
            function unAssignStudentAction(groupId, studentId) {
                if (!studentId)
                    return null;
                var res = this.groupService
                    .unassignStudents(groupId, [studentId])
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId]],
            function assignSchoolAction(groupId, schoolYearId) {
                var res = this.groupService
                    .assignSchool(groupId, schoolYearId)
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId]],
            function unAssignSchoolAction(groupId, schoolYearId) {
                var res = this.groupService
                    .unassignSchool(groupId, schoolYearId)
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId]],
            function assignAllSchoolsAction(groupId) {
                var res = this.groupService
                    .assignAllSchools(groupId)
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId]],
            function unAssignAllSchoolsAction(groupId) {
                var res = this.groupService
                    .unassignAllSchools(groupId)
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.GradeLevelId, chlk.models.id.SchoolYearId, String]],
            function assignAllStudentsAction(groupId, gradeLevelId, schoolYearId, studentIds) {
                if (!studentIds)
                    return null;

                var res = this.groupService
                    .assignStudents(groupId, this.getIdsList(studentIds, chlk.models.id.SchoolPersonId))
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.GradeLevelId, chlk.models.id.SchoolYearId, String]],
            function unAssignAllStudentsAction(groupId, gradeLevelId, schoolYearId, studentIds) {
                if (!studentIds)
                    return null;

                var res = this.groupService
                    .unassignStudents(groupId, this.getIdsList(studentIds, chlk.models.id.SchoolPersonId))
                    .then(function (model) {
                        return new chlk.models.Success();
                    })
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[String, chlk.models.id.AnnouncementId]],
            function editGroupsAction(listRequestId, announcementId_) {
                var res = this.groupService.list()
                    .then(function (groups) {
                        return new chlk.models.group.GroupsListViewData(listRequestId, groups, announcementId_);
                    })
                    .attach(this.validateResponse_());

                return this.ShadeView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[String, chlk.models.id.GroupId, chlk.models.id.AnnouncementId]],
            function tryDeleteGroupAction(listRequestId, groupId, announcementId_) {
                this.ShowConfirmBox('Are you sure you want to delete this group?', "whoa.", null, 'negative-button')
                    .then(function (data) {
                        return this.BackgroundNavigate('group', 'deleteGroup', [listRequestId, groupId, announcementId_]);
                    }, this);
                return null;
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[String, chlk.models.id.GroupId, chlk.models.id.AnnouncementId]],
            function deleteGroupAction(listRequestId, groupId, announcementId_) {
                var res = this.groupService
                    .deleteGroup(groupId)
                    .then(function (groups) {
                        this.afterGroupEdit_(listRequestId, groups, announcementId_);
                        return new chlk.models.group.GroupsListViewData(listRequestId, groups, announcementId_);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

            /*[chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.group.Group]],
            function createGroupAction(model) {
                var res = this.groupService
                    .create(model.getName())
                    .then(function (groups) {
                        this.afterGroupEdit_(model.getListRequestId(), groups, model.getAnnouncementId());
                        return new chlk.models.group.GroupsListViewData(model.getListRequestId(), groups, model.getAnnouncementId());
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },*/

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.group.Group]],
            function editGroupNameAction(model) {
                var res = this.groupService
                    .editName(model.getId(), model.getName())
                    .then(function (groups) {
                        this.afterGroupEdit_(model.getListRequestId(), groups, model.getAnnouncementId());
                        return new chlk.models.group.GroupsListViewData(model.getListRequestId(), groups, model.getAnnouncementId());
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
            function selectStudentFiltersAction(groupId, schoolYearId, gradeLevelId) {
                var res = this.classService
                    .detailedCourseTypes(schoolYearId, gradeLevelId)
                    .then(function (courseTypes) {
                        return new chlk.models.announcement.GroupStudentsFilterViewData(groupId, schoolYearId, gradeLevelId, courseTypes)
                    })
                    .attach(this.validateResponse_());

                return this.ShadeView(chlk.activities.announcement.GroupStudentsFilterDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.announcement.GroupStudentsFilterViewData]],
            function filterStudentsAction(model) {
                this.BackgroundCloseView(chlk.activities.announcement.GroupStudentsFilterDialog);
                return this.Redirect('group', 'showGradeLevelMembers', [model.getGroupId(), model.getSchoolYearId(), model.getGradeLevelId(), model.getClassIds()]);
            },

            function afterGroupEdit_(requestId, groups, announcementId_) {
                var groupsIds = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_IDS, []);
                var model = new chlk.models.group.AnnouncementGroupsViewData(requestId, groups, announcementId_, groupsIds, {
                    id: announcementId_
                });
                this.getContext().getSession().set(ChlkSessionConstants.GROUPS_LIST, groups);
                this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, model);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[String, chlk.models.id.AnnouncementId]],
            function addGroupAction(listRequestId_, announcementId_) {
                var group = new chlk.models.group.Group();
                announcementId_ && group.setAnnouncementId(announcementId_);
                listRequestId_ && group.setListRequestId(listRequestId_);
                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, new ria.async.DeferredData(group));
            }
        ]);

});