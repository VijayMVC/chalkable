REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.chlkerror.Error404Tpl');

NAMESPACE('chlk.activities.chlkerror', function () {

    /** @class chlk.activities.chlkerror.Error404Page */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.chlkerror.Error404Tpl)],
        'Error404Page', EXTENDS(chlk.activities.lib.TemplatePage), []);
});