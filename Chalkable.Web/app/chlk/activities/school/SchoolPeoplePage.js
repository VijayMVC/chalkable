REQUIRE('chlk.activities.TemplatePage');
REQUIRE('chlk.templates.school.SchoolPeople');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolPeoplePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.PageClass('profile')],
        [chlk.activities.BindTemplate(chlk.templates.school.SchoolPeople)],
        'SchoolPeoplePage', EXTENDS(chlk.activities.TemplatePage), [ ]);
});