REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.storage.DbMaintenance');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.DbMaintenance*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.storage.DbMaintenance)],
        'DbMaintenancePage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});