REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AddAppDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/add-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.Application)],
        'AddAppDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'name'
        ])
});