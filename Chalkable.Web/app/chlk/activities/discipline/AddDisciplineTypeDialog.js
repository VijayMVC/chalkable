REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.discipline.AddDisciplineTypeTpl');


NAMESPACE('chlk.activities.discipline', function(){
    "use strict";

    /**@class chlk.activities.discipline.AddDisciplineTypeDialog*/

    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.AddDisciplineTypeTpl)],
        'AddDisciplineTypeDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});