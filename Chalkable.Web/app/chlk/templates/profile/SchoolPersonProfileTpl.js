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
                //this._adminContorllerName = 'districtadmins';
                this._teacherControllerName = 'teachers';
                this._studentControllerName = 'students';
            },

            [ria.templates.ModelPropertyBind],
            String, 'title',

            TUser, function getUser(){
                return this.getModel().getUser();},

            String, function getControllerName(){
                var roleEnums = chlk.models.common.RoleEnum;
                var roleId = this.getUser().getRole().getId();
                //if(roleId == roleEnums.DISTRICTADMIN.valueOf()){
                //    return this._adminContorllerName;
                //}
                if(roleId == roleEnums.TEACHER.valueOf()
                    || roleId == roleEnums.DISTRICTADMIN.valueOf())
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

            [[Object, String]],
            Object, function getProfileScheduleTplParams(user, currentAction){
                var role = user.getRole();
                var scheduleActions = {
                    day: 'daySchedule',
                    week: 'weekSchedule',
                    month: 'monthSchedule',
                    currentAction: currentAction
                };
                return {
                    controllerName: this.getControllerName(),
                    actions: scheduleActions
                };
            },

            [[String]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
                var controller = this.getControllerName();
                var userId = this.getUser().getId().valueOf();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var isStudentController = controller == this._studentControllerName;
                var canViewInfo = userId == this.getCurrentUser().getId().valueOf()
                           || (isStudentController && !this.getUserRole().isStudent() && this.hasUserPermission_(permissionEnum.VIEW_ADDRESS));

                var res = [
                    this.createActionLinkModel_(controller, 'details', 'Now', pressedLinkName, [userId]),
                    this.createActionLinkModel_(controller, 'info', 'Info', pressedLinkName, [userId], !canViewInfo)
//                    ,  (!isStudentController && this.getUserRole().isStudent())
//                     || (isStudentController && !this.hasUserPermission_(permissionEnum.VIEW_ADDRESS)))
                ];
                if(isStudentController){
                    res.push(this.createActionLinkModel_(controller, 'grading', 'Grading', pressedLinkName, [userId], false));
                }
                if(controller == this._teacherControllerName || controller == this._studentControllerName){
                    res.push(this.createActionLinkModel_(controller, 'daySchedule', 'Schedule', pressedLinkName, [userId], false));
                }
                if(isStudentController){
                    res.push(this.createActionLinkModel_(controller, 'attendance', 'Attendance'
                        , pressedLinkName, [null, userId], !this.hasUserPermission_(permissionEnum.VIEW_ATTENDANCE)));
                    res.push(this.createActionLinkModel_(controller, 'discipline', 'Discipline'
                        , pressedLinkName, [null, userId], !this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_DISCIPLINE)));
                    res.push(this.createActionLinkModel_(controller, 'explorer', 'Explorer', pressedLinkName, [userId], !this.isStudyCenterEnabled() || this.getUserRole().isAdmin()));
                    
                    res.push(this.createActionLinkModel_(controller, 'apps', 'Apps', pressedLinkName, [userId], false));
                }
                else
                    res.push(this.createActionLinkModel_(controller, 'apps', 'Apps', pressedLinkName, [userId], true));

                if(isStudentController)
                    res.push(this.createActionLinkModel_(this._studentControllerName, 'assessmentProfile', 'Assessments', pressedLinkName, [userId], !this.isAssessmentEnabled()));
                return res;
                //return this.getModel().getActionLinksData(); // todo : rewrite this method
            }
    ]);
});