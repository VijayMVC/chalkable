REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.SchoolDetails');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolDetailsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [chlk.activities.lib.BindTemplate(chlk.templates.school.SchoolDetails)],
        'SchoolDetailsPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});