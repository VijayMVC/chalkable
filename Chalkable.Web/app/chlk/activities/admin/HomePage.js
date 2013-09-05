REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.admin.HomePage');

NAMESPACE('chlk.activities.admin', function () {

    /** @class chlk.activities.admin.HomePage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.admin.HomePage)],
        'HomePage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});