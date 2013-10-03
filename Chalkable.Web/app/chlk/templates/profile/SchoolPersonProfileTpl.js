REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.people.UserProfileViewData');

NAMESPACE('chlk.templates.profile', function(){
   "use strict";

    /**@class chlk.templates.profile.SchoolPersonProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.people.UserProfileViewData)],
        'SchoolPersonProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

            function $(){
                BASE();
                this._adminContorllerName = 'admins';
                this._teacherControllerName = 'teachers';
                this._studentControllerName = 'students';
            },

            Object, function getUser(){
                console.log(this.getModel().getUser());
                return this.getModel().getUser();},

            String, function getControllerName(){
                var roleEnums = chlk.models.common.RoleEnum;
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

            [[String]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
                var controller = this.getControllerName();
                console.log(this.getUser());
                var userId = this.getUser().getId().valueOf();
                var res = [
                    this.createActionLinkModel_(controller, 'details', 'Now', pressedLinkName, [userId]),
                    this.createActionLinkModel_(controller, 'info', 'Info', pressedLinkName, [userId])
                ];
                if(controller == this._studentControllerName){
                    res.push(this.createActionLinkModel_(controller, 'grading', 'Grading', pressedLinkName, [userId]));
                }
                if(controller == this._teacherControllerName || controller == this._studentControllerName){
                    res.push(this.createActionLinkModel_(controller, 'schedule', 'Schedule', pressedLinkName, [userId]));
                }
                if(controller == this._studentControllerName){
                    res.push(this.createActionLinkModel_(controller, 'attendance', 'Attendance', pressedLinkName, [null, userId]));
                    res.push(this.createActionLinkModel_(controller, 'discipline', 'Discipline', pressedLinkName, [null, userId]));
                }
                res.push(this.createActionLinkModel_(controller, 'apps', 'Apps', pressedLinkName, [userId]));
                return res;
                //return this.getModel().getActionLinksData(); // todo : rewrite this method
            }
    ]);
});