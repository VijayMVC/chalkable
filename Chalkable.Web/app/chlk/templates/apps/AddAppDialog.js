REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AddAppDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/add-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.Application)],
        'AddAppDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'name'
        ])
});