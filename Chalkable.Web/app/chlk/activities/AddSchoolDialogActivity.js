REQUIRE('chlk.activities.TemplateActivity');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.Schools */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [chlk.activities.BindTemplate(chlk.templates.School)],
        'SchoolsActivity', EXTENDS(chlk.activities.TemplateActivity), []);
});