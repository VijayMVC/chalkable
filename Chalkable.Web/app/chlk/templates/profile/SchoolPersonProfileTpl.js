REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.people.UserProfileViewData');

NAMESPACE('chlk.templates.profile', function(){
   "use strict";

    /**@class chlk.templates.profile.SchoolPersonProfileTpl*/
    CLASS(
        GENERIC('TUser', ClassOf(chlk.models.people.ShortUserInfo)),
        [ria.templates.ModelBind(chlk.models.people.UserProfileViewData.OF(TUser))],
        'SchoolPersonProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

            function $(){
                BASE();
                this._adminContorllerName = 'admins';
                this._teacherControllerName = 'teachers';
                this._studentControllerName = 'students';
            },

            TUser, function getUser(){
                return this.getModel().getUser();},

            String, function getControllerName(){
                var roleEnums = chlk.models.common.RoleEnum;
                console.log(this.getUser().getRole());
                var roleId = this.getUser().getRole().getId();
                if(roleId == roleEnums.ADMINGRADE.valueOf()
                    || roleId == roleEnums.ADMINEDIT.valueOf()
                    || roleId == roleEnums.ADMINVIEW.valueOf()){
                    return this._adminContorllerName;
                }
                if(roleId == roleEnums.TEACHER.valueOf())
                    return this._teacherControllerName;
                if(roleId == roleEnums.STUDENT.valueOf())
                    return this._studentControllerName;
            },

            [[Object, Number]],
            String, function getPersonEnrollmentStatus(user, classesNumber){
                var result = '';
                var role = user.getRole();

                if (role.getId()){
                    result = "Currently ";
                    if (classesNumber > 0){
                        result += role.getId() == chlk.models.common.RoleEnum.TEACHER ? 'teaching ' : 'enrolled in ';
                        result += classesNumber;
                        result += classesNumber > 1 ? ' classes' : ' class';
                    }
                    else{
                        result += role.getId() == chlk.models.common.RoleEnum.TEACHER ? 'doesn\'t teach any class' : 'unenrolled';
                    }
                }
                return result;
            },

            [[String]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
                var controller = this.getControllerName();
                var userId = this.getUser().getId().valueOf();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var isStudentController = controller == this._studentControllerName;
                var canViewInfo = userId == this.getCurrentUser().getId().valueOf()
                    || (!this.getUserRole().isStudent() && this.hasUserPermission_(permissionEnum.VIEW_ADDRESS));
                var res = [
                    this.createActionLinkModel_(controller, 'details', 'Now', pressedLinkName, [userId]),
                    this.createActionLinkModel_(controller, 'info', 'Info', pressedLinkName, [userId], !canViewInfo)
//                    ,  (!isStudentController && this.getUserRole().isStudent())
//                     || (isStudentController && !this.hasUserPermission_(permissionEnum.VIEW_ADDRESS)))
                ];
                if(isStudentController){
                    res.push(this.createActionLinkModel_(controller, 'grading', 'Grading', pressedLinkName, [userId], true));
                }
                if(controller == this._teacherControllerName || controller == this._studentControllerName){
                    res.push(this.createActionLinkModel_(controller, 'schedule', 'Schedule', pressedLinkName, [userId], false));
                }
                if(isStudentController){
                    res.push(this.createActionLinkModel_(controller, 'attendance', 'Attendance'
                        , pressedLinkName, [null, userId], true));//!this.hasUserPermission_(permissionEnum.VIEW_ATTENDANCE)));
                    res.push(this.createActionLinkModel_(controller, 'discipline', 'Discipline'
                        , pressedLinkName, [null, userId], true));//!this.hasUserPermission_(permissionEnum.VIEW_DISCIPLINE)));
                }
                res.push(this.createActionLinkModel_(controller, 'apps', 'Apps', pressedLinkName, [userId], true));
                return res;
                //return this.getModel().getActionLinksData(); // todo : rewrite this method
            }
    ]);
});