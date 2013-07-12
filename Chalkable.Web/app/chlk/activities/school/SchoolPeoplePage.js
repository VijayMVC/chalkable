REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.SchoolPeople');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolPeoplePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [chlk.activities.lib.BindTemplate(chlk.templates.school.SchoolPeople)],
        'SchoolPeoplePage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});