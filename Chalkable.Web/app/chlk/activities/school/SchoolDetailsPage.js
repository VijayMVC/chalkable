REQUIRE('chlk.activities.TemplatePage');
REQUIRE('chlk.templates.school.SchoolDetails');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolDetailsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.PageClass('profile')],
        [chlk.activities.BindTemplate(chlk.templates.school.SchoolDetails)],
        'SchoolDetailsPage', EXTENDS(chlk.activities.TemplatePage), [ ]);
});