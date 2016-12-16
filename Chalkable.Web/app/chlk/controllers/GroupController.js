REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.GroupService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.TeacherService');

REQUIRE('chlk.models.recipients.RecipientsSubmitViewData');
REQUIRE('chlk.models.group.AnnouncementGroupsViewData');

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
                        this.studentService.getStudents(classId, null, !isStudent, null, 0, itemsCount, true),
                        isStudent ? this.teacherService.getTeachers(classId, null, null, 0, itemsCount, true) :
                            this.studentService.getStudents(classId, null, false, null, 0, itemsCount, true),
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
            function deleteGroupAction(groupId) {
                var res = this.groupService
                    .deleteGroup(groupId)
                    .then(function (groups) {
                        var recipients = this.getContext().getSession().get(ChlkSessionConstants.ADMIN_RECIPIENTS, null);
                        if(recipients){
                            recipients.groups = recipients.groups.filter(function(group){return group.getId() != groupId});
                            this.getContext().getSession().set(ChlkSessionConstants.ADMIN_RECIPIENTS, recipients);
                        }

                        this.BackgroundUpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, groupId, 'remove-group');
                        return ria.async.BREAK;
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(this.getView().getCurrent().getClass(), res);
            }
        ]);

});