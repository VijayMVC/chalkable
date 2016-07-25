REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.chlkerror.GeneralErrorTpl');

NAMESPACE('chlk.activities.chlkerror', function () {

    /** @class chlk.activities.chlkerror.GeneralServerErrorPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.chlkerror.GeneralErrorTpl)],
        'GeneralServerErrorPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});