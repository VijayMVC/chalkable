REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.Hello');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.HelloPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.Hello)],
        'HelloPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});