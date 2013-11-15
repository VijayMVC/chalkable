REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.MyApps');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.MyAppsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.MyApps)],
        [chlk.activities.lib.PartialUpdateClass('partial-update-myapps')],
        'MyAppsPage', EXTENDS(chlk.activities.lib.TemplatePage), [
        ]);
});