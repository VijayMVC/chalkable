REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.storage.DbMaintenance');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.DbMaintenance*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.storage.DbMaintenance)],
        'DbMaintenancePage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});