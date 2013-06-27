REQUIRE('chlk.activities.TemplateDialog');
REQUIRE('chlk.templates.AddSchool.Dialog');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.AddSchoolDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [chlk.activities.BindTemplate(chlk.templates.AddSchool.Dialog)],
        'AddSchoolDialog', EXTENDS(chlk.activities.TemplateDialog), [
            [[Object]],
            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

            }
        ]);
});