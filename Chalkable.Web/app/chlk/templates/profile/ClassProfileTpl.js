REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.common.ActionLinkModel');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    /**@class chlk.templates.profile.ClassProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

            Object, function getClazz(){
                return this.getModel().getClazz();
            },

            [[String, chlk.models.id.ClassId]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedActionName, classId_){

                if(!classId_ && (this.getModel() instanceof chlk.models.classes.BaseClassProfileViewData)){
                    classId_ = this.getClazz().getId();
                }
                var userRole = this.getUserRole();
                var isAdminOrTeacher = userRole.isAdmin() || userRole.isTeacher();
                var links = [];

                if (isAdminOrTeacher)
                //!this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ROSTER)),
                    links.push(this.buildActionLinkModelForClass('details', 'Now', pressedActionName, classId_, true));

                links.push(this.buildActionLinkModelForClass('info', 'Info', pressedActionName, classId_));

                if (isAdminOrTeacher){
                    //!isAdminOrTeacher && !this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ATTENDANCE)),
                    links.push(this.buildActionLinkModelForClass('attendance', 'Attendance', pressedActionName, classId_, true));
                    links.push(this.buildActionLinkModelForClass('apps', 'Apps', pressedActionName, classId_, true));
                    links.push(this.buildActionLinkModelForClass('schedule', 'Schedule', pressedActionName, classId_, true));
                    //!this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_GRADES))
                    links.push(this.buildActionLinkModelForClass('grading', 'Grading', pressedActionName, classId_, true));
                }
                return links;
            },

            [[String, String, String, chlk.models.id.ClassId]],
            chlk.models.common.ActionLinkModel, function buildActionLinkModelForClass(action, title, pressedActionName, classId_, disabled_){
                return new chlk.models.common.ActionLinkModel('class', action, title, pressedActionName == action
                    , [classId_ || null], null, disabled_);
            }
    ]);
});