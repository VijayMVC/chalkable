REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.discipline.SetDisciplineDialog');

NAMESPACE('chlk.activities.discipline', function(){

    /** @class chlk.activities.discipline.SetDisciplineDialog */

    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.SetDisciplineDialog)],

        'SetDisciplineDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});