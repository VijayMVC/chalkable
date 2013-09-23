REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.admin.PersonsForAdmin');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.templates.admin', function(){
    "use strict";

    /**@class chlk.templates.admin.SchoolPersonListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/admin/school-person-list.jade')],
        [ria.templates.ModelBind(chlk.models.admin.PersonsForAdmin)],
        'SchoolPersonListTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.people.UsersList, 'usersList',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.Role), 'roles',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels'
    ]);
});