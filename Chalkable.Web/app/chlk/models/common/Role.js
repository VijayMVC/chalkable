NAMESPACE('chlk.models.common', function () {
    "use strict";


    /** @class chlk.models.common.RoleEnum*/
    ENUM(
        'RoleEnum', {
            SYSADMIN: 1,
            ADMINVIEW: 8,
            ADMINGRADE: 5,
            ADMINEDIT: 7,
            TEACHER: 2,
            STUDENT: 3,
            PARENT: 4,
            DEVELOPER: 9,
            CHECKIN: 6
        });

    /** @class chlk.models.common.RoleNamesEnum*/
    ENUM(
        'RoleNamesEnum', {
            SYSADMIN: 'sysadmin',
            ADMINVIEW: 'adminview',
            ADMINGRADE: 'admingrade',
            ADMINEDIT: 'adminedit',
            TEACHER: 'teacher',
            STUDENT: 'student',
            PARENT: 'parent',
            DEVELOPER: 'developer',
            CHECKIN: 'checkin'
        });

    /** @class chlk.models.common.Role*/
    CLASS(
        'Role', [
            chlk.models.common.RoleEnum, 'roleId',
            String, 'roleName',
            [[chlk.models.common.RoleEnum, String]],
            function $(roleId, roleName){
                BASE();
                this.setRoleId(roleId);
                this.setRoleName(roleName);
            }
        ]);
});
