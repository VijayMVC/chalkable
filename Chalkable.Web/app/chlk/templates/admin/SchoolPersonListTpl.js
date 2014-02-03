REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.admin.PersonsForAdmin');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.templates.admin', function(){
    "use strict";

    /**@class chlk.templates.admin.SchoolPersonListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/admin/school-person-list.jade')],
        [ria.templates.ModelBind(chlk.models.admin.PersonsForAdmin)],
        'SchoolPersonListTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.people.UsersList, 'usersList',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.Role), 'roles',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',

            OVERRIDE, chlk.models.common.Role, function getUserRole(){
                return new chlk.models.common.Role(chlk.models.common.RoleEnum.ADMINGRADE, chlk.models.common.RoleNamesEnum.ADMINGRADE.valueOf());
            }
    ]);
});