REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.SchoolPeople');
REQUIRE('chlk.templates.school.SchoolPeoplePart');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolPeoplePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.school.SchoolPeople)],
        'SchoolPeoplePage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});