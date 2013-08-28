REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.storage.DbMaintenance');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.DbMaintenance*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.storage.DbMaintenance)],
        [ria.mvc.PartialUpdateRule(chlk.templates.storage.DbMaintenance, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'DbMaintenancePage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});