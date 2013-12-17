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
            function $(roleId_, roleName_){
                BASE();
                if(roleId_)
                    this.setRoleId(roleId_);
                if(roleName_)
                    this.setRoleName(roleName_);
            },

            Boolean, function isAdmin(){
                var roleEnums = chlk.models.common.RoleEnum;
                var roleId = this.getRoleId();
                return roleId == roleEnums.ADMINGRADE
                    || roleId == roleEnums.ADMINEDIT
                    || roleId == roleEnums.ADMINVIEW
            },

            Boolean, function isTeacher(){
                var roleEnums = chlk.models.common.RoleEnum;
                var roleId = this.getRoleId();
                return roleId == roleEnums.TEACHER;
            },

            Boolean, function isStudent(){
                var roleEnums = chlk.models.common.RoleEnum;
                var roleId = this.getRoleId();
                return roleId == roleEnums.STUDENT;
            }
        ]);
});
