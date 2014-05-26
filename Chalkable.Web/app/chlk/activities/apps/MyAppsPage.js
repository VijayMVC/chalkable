REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.MyApps');
REQUIRE('chlk.templates.apps.MyAppsSearchBoxTpl');
REQUIRE('chlk.templates.SuccessTpl');


NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.MyAppsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.MyApps)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.MyApps, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [chlk.activities.lib.PartialUpdateClass('partial-update-myapps')],
        'MyAppsPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doNothing(tpl, model, msg_) {

            }
        ]);
});