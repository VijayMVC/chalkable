REQUIRE('chlk.models.apps.AppMarketInstallViewData');
REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.InstallAppDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/install-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketInstallViewData)],
        'InstallAppDialogTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'app',

            [ria.templates.ModelPropertyBind],
            ArrayOf(ArrayOf(chlk.models.apps.AppInstallGroup)), 'installGroupColumns',


            [ria.templates.ModelPropertyBind],
            Boolean, 'alreadyInstalled',

            [ria.templates.ModelPropertyBind],
            String, 'installGroupColumnWidth'
        ])
});