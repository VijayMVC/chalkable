REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppInfo');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppInfoPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppInfo)],
        'AppInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});