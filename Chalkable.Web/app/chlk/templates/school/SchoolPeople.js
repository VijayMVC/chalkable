REQUIRE('chlk.models.school.SchoolDetails');
REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.SchoolPeople*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolPeople.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolPeople)],
        'SchoolPeople', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.people.User), 'users',
            [ria.templates.ModelBind],
            chlk.models.school.SchoolDetails, 'schoolInfo'
        ])
});
