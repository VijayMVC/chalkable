REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.common.ActionLinkModel');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    /**@class chlk.templates.profile.ClassProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

            [ria.templates.ModelPropertyBind],
            Boolean, 'assignedToClass',

            Object, function getClazz(){
                return this.getModel().getClazz();
            },

            [[String, chlk.models.id.ClassId]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedActionName, classId_){

                var teacherIds = [];
                if(!classId_ && (this.getModel() instanceof chlk.models.classes.BaseClassProfileViewData)){
                    classId_ = this.getClazz().getId();
                    teacherIds = this.getClazz().getTeachersIds();
                }
                var userRole = this.getUserRole();
                var isAdminOrTeacher = userRole.isAdmin() || userRole.isTeacher();
                var links = [];

                if (isAdminOrTeacher)
                //!this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ROSTER)),
                    links.push(this.buildActionLinkModelForClass('details', 'Now', pressedActionName, classId_, !this.canViewSummary_(teacherIds)));

                links.push(this.buildActionLinkModelForClass('info', 'Info', pressedActionName, classId_));



                if (isAdminOrTeacher){
                    links.push(this.buildActionLinkModelForClass('attendance', 'Attendance', pressedActionName, classId_, !this.canViewAttendance_(teacherIds), '7.1.6.19573'));
                    links.push(this.buildActionLinkModelForClass('discipline', 'Discipline', pressedActionName, classId_, !this.canViewDiscipline_(teacherIds), '7.1.6.19573'));
                    links.push(this.buildActionLinkModelForClass('apps', 'Apps', pressedActionName, classId_, true));
                    links.push(this.buildActionLinkModelForClass('schedule', 'Schedule', pressedActionName, classId_, !this.canViewSchedule_()));
                    //!this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_GRADES))
                    links.push(this.buildActionLinkModelForClass('grading', 'Grading', pressedActionName, classId_, !this.canViewGrading_(teacherIds), '7.1.6.19573'));
                    links.push(this.buildActionLinkModelForClass('explorer', 'Explorer', pressedActionName, classId_, !this.canViewExplorer_(teacherIds)));
                    links.push(this.buildActionLinkModelForClass('panorama', 'Panorama', pressedActionName, classId_, !this.canViewPanorama_(teacherIds)));
                }
                return links;
            },

            [[ArrayOf(chlk.models.id.SchoolPersonId)]],
            Boolean, function canViewSummary_(teacherIds){
                var currentUserId = this.getCurrentUser().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var canViewSummary = this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ADMIN)
                    || (this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM)
                    && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0);
                return canViewSummary;
            },

            [[ArrayOf(chlk.models.id.SchoolPersonId)]],
            Boolean, function canViewGrading_(teacherIds){
                var currentUserId = this.getCurrentUser().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var canViewSummary = this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ADMIN)
                    || (this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM)
                    && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0);
                return canViewSummary;
            },

            [[ArrayOf(chlk.models.id.SchoolPersonId)]],
            Boolean, function canViewDiscipline_(teacherIds){
                var currentUserId = this.getCurrentUser().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var canViewSummary = this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN)
                    || (this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_DISCIPLINE)
                    && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0);
                return canViewSummary;
            },

            Boolean, function canViewSchedule_(){
                var res = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM);
                return res && this.isAssignedToClass() || this.getUserRole().isStudent();
            },

            [[ArrayOf(chlk.models.id.SchoolPersonId)]],
            Boolean, function canViewAttendance_(teacherIds){
                var currentUserId = this.getCurrentUser().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var canViewAttendance = this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN)
                    || (this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ATTENDANCE)
                    && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0);
                return canViewAttendance;
            },

            [[ArrayOf(chlk.models.id.SchoolPersonId)]],
            Boolean, function canViewExplorer_(teacherIds){
                var currentUserId = this.getCurrentUser().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var canViewExplorer = this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ADMIN)
                    || (this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM)
                    && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0);
                return canViewExplorer && this.isStudyCenterEnabled();
            },

            [[ArrayOf(chlk.models.id.SchoolPersonId)]],
            Boolean, function canViewPanorama_(teacherIds){
                var currentUserId = this.getCurrentUser().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var canViewPanorama = this.getUserRole().isAdmin() && this.hasUserPermission_(permissionEnum.VIEW_PANORAMA)
                    || this.hasUserPermission_(permissionEnum.VIEW_PANORAMA) && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0;
                return canViewPanorama && this.isStudyCenterEnabled();
            },

            [[String, String, String, chlk.models.id.ClassId]],
            chlk.models.common.ActionLinkModel, function buildActionLinkModelForClass(action, title, pressedActionName, classId_, disabled_, sisApiVersion_){
                return new chlk.models.common.ActionLinkModel('class', action, title, pressedActionName == action
                    , [classId_ || null], null, disabled_, sisApiVersion_);
            }
    ]);
});