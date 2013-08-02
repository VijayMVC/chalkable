NAMESPACE('chlk.models.common', function () {
    "use strict";


    /** @class chlk.models.common.RoleEnum*/
    ENUM(
        'RoleEnum', {
            SYSADMIN: 1,
            ADMINVIEW: 2,
            ADMINGRADE: 3,
            ADMINEDIT: 4,
            TEACHER: 5,
            STUDENT: 6,
            PARENT: 7,
            DEVELOPER: 8
        });

    /** @class chlk.models.common.Role*/
    CLASS(
        'Role', [
            chlk.models.common.RoleEnum, 'roleId',
            String, 'roleName',
            [[chlk.models.common.RoleEnum, String]],
            function $(roleId, roleName){
                this.setRoleId(roleId);
                this.setRoleName(roleName);
            }
        ]);
});
