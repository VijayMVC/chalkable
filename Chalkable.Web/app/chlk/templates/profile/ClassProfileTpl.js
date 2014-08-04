REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.common.ActionLinkModel');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    /**@class chlk.templates.profile.ClassProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

            Object, function getClazz(){return this.getModel().getClazz();},

            [[String, chlk.models.id.ClassId]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedActionName, classId_){

                if(!classId_ && (this.getModel() instanceof chlk.models.classes.BaseClassProfileViewData)){
                    classId_ = this.getClazz().getId();
                }
                var role = this.getCurrentRoleId();
                var isAdminOrTeacher = this.isAdmin()
                    || role == chlk.models.common.RoleEnum.TEACHER;

                var permissionEnum = chlk.models.people.UserPermissionEnum;
                return [
                    this.buildActionLinkModelForClass('details', 'Now', pressedActionName, classId_, true),//!this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ROSTER)),
                    this.buildActionLinkModelForClass('info', 'Info', pressedActionName, classId_),
                    this.buildActionLinkModelForClass('schedule', 'Schedule', pressedActionName, classId_, true),
                    this.buildActionLinkModelForClass('grading', 'Grading', pressedActionName, classId_, true), //!this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_GRADES)),
                    this.buildActionLinkModelForClass('attendance', 'Attendance', pressedActionName, classId_, true), //!isAdminOrTeacher && !this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ATTENDANCE)),
                    this.buildActionLinkModelForClass('apps', 'Apps', pressedActionName, classId_, true)
                ]
            },

            [[String, String, String, chlk.models.id.ClassId]],
            chlk.models.common.ActionLinkModel, function buildActionLinkModelForClass(action, title, pressedActionName, classId_, disabled_){
                return new chlk.models.common.ActionLinkModel('class', action, title, pressedActionName == action
                    , [classId_ || null], null, disabled_);
            }
    ]);
});