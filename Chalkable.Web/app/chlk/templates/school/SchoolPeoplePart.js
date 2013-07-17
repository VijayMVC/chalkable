REQUIRE('chlk.templates.school.SchoolPeople');
REQUIRE('chlk.models.school.SchoolPeople');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolPeoplePart*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolPeoplePart.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolPeople)],
        'SchoolPeoplePart', EXTENDS(chlk.templates.school.SchoolPeople), [])
});
