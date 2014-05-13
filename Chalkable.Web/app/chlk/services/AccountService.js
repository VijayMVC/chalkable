REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.SimpleResult');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AccountService */
    CLASS(
        'AccountService', EXTENDS(chlk.services.BaseService), [
            ArrayOf(chlk.models.common.Role), function getSchoolRoles() {
                return [
                    new chlk.models.common.Role(chlk.models.common.RoleEnum.ADMINGRADE, chlk.models.common.RoleNamesEnum.ADMINGRADE.valueOf()),
                    new chlk.models.common.Role(chlk.models.common.RoleEnum.ADMINEDIT, chlk.models.common.RoleNamesEnum.ADMINEDIT.valueOf()),
                    new chlk.models.common.Role(chlk.models.common.RoleEnum.ADMINVIEW, chlk.models.common.RoleNamesEnum.ADMINVIEW.valueOf()),
                    new chlk.models.common.Role(chlk.models.common.RoleEnum.TEACHER, chlk.models.common.RoleNamesEnum.TEACHER.valueOf()),
                    new chlk.models.common.Role(chlk.models.common.RoleEnum.STUDENT, chlk.models.common.RoleNamesEnum.STUDENT.valueOf())
                ];
            },

            ria.async.Future, function logOut() {
                return this.post('User/LogOut.json', chlk.models.common.SimpleResult, {});
            },

//            ria.async.Future, function redirectToINOW(){
//                return this.post('User/RedirectToINow.json', chlk.models.common.SimpleResult,{});
//            },

            [[String,String,String]],
            ria.async.Future, function changePassword(oldPassword, newPassword, newPasswordConfirmation) {
                return this.get('user/ChangePassword.json', Boolean
                    , {
                        oldPassword: oldPassword,
                        newPassword: newPassword,
                        newPasswordConfirmation: newPasswordConfirmation
                    });
            }


            //doChangePassword(model.oldPassword, model.newPassword, model.newPasswordConfirmation)
        ])
});