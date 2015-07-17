REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.GroupService');

REQUIRE('chlk.activities.announcement.GroupStudentsFilterDialog');

NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.GroupController */
    CLASS(
        [chlk.controllers.NotChangedSidebarButton],
        'GroupController', EXTENDS(chlk.controllers.BaseController), [
            [ria.mvc.Inject],
            chlk.services.GroupService, 'groupService',

            [[chlk.models.group.AnnouncementGroupsViewData]],
            function showAction(model) {
                var res = this.groupService.list()
                    .then(function(groups){
                        this.getContext().getSession().set(ChlkSessionConstants.GROUPS_LIST, groups);
                        model.setGroups(groups);
                        return model;
                    }, this)
                    .attach(this.validateResponse_());

                return this.ShadeView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

            [[chlk.models.id.GroupId]],
            function showGroupMembersAction(groupId) {
                var res = this.groupService
                    .groupExplorer(groupId)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
            },

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

            function editGroupsAction() {
                var res = this.groupService.list()
                    .then(function (groups) {
                        return new chlk.models.group.GroupsListViewData(groups);
                    })
                    .attach(this.validateResponse_());

                return this.ShadeView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

            [[chlk.models.id.GroupId]],
            function tryDeleteGroupAction(groupId) {
                this.ShowConfirmBox('Are you sure you want to delete this group?', "whoa.", null, 'negative-button')
                    .then(function (data) {
                        return this.BackgroundNavigate('group', 'deleteGroup', [groupId]);
                    }, this);
                return null;
            },

            [[chlk.models.id.GroupId]],
            function deleteGroupAction(groupId) {
                var res = this.groupService
                    .deleteGroup(groupId)
                    .then(function (groups) {
                        this.afterGroupEdit_(groups);
                        return new chlk.models.group.GroupsListViewData(groups);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

            [[chlk.models.group.Group]],
            function createGroupAction(model) {
                var res = this.groupService
                    .create(model.getName())
                    .then(function (groups) {
                        this.afterGroupEdit_(groups);
                        return new chlk.models.group.GroupsListViewData(groups);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

            [[chlk.models.group.Group]],
            function editGroupNameAction(model) {
                var res = this.groupService
                    .editName(model.getId(), model.getName())
                    .then(function (groups) {
                        this.afterGroupEdit_(groups);
                        return new chlk.models.group.GroupsListViewData(groups);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            },

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

            [[chlk.models.announcement.GroupStudentsFilterViewData]],
            function filterStudentsAction(model) {
                this.BackgroundCloseView(chlk.activities.announcement.GroupStudentsFilterDialog);
                return this.Redirect('group', 'showGradeLevelMembers', [model.getGroupId(), model.getSchoolYearId(), model.getGradeLevelId(), model.getClassIds()]);
            },

            function afterGroupEdit_(groups) {
                this.getContext().getSession().set(ChlkSessionConstants.GROUPS_LIST, groups);
                this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, groups, 'after-edit');
            },

            function addGroupAction() {
                var res = new ria.async.DeferredData(new chlk.models.group.Group);

                return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
            }
        ]);

});