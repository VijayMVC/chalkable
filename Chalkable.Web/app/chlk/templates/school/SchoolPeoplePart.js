REQUIRE('chlk.templates.people.UsersListTpl');
REQUIRE('chlk.models.school.SchoolPeoplePart');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolPeoplePart*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/people/UsersList.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolPeoplePart)],
        'SchoolPeoplePart', EXTENDS(chlk.templates.people.UsersListTpl), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'schoolId'
        ])
});
