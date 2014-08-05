REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.discipline.DisciplineTypeListTpl');


NAMESPACE('chlk.activities.discipline', function(){
    "use strict";

    /** @class chlk.activities.discipline.DisciplineTypePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.DisciplineTypeListTpl)],
        'DisciplineTypePage', EXTENDS(chlk.activities.lib.TemplatePage),[]);
});