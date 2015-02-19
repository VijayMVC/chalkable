REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.Start');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.StartPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.Start)],
        'StartPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});