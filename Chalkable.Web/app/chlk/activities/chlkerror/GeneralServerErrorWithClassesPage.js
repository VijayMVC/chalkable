REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.chlkerror.GeneralServerErrorWithClassesTpl');

NAMESPACE('chlk.activities.chlkerror', function () {

    /** @class chlk.activities.chlkerror.GeneralServerErrorWithClassesPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.chlkerror.GeneralServerErrorWithClassesTpl)],
        'GeneralServerErrorWithClassesPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});