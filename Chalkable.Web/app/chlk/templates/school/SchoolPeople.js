REQUIRE('chlk.models.school.SchoolPeopleSummary');
REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.models.people.UsersList');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolPeople*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolPeople.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolPeople)],
        'SchoolPeople', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.UsersList, 'usersList', //todo: rename
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'roles',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo'
        ])
});
