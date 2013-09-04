REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.apps.AppWrapperViewData');
REQUIRE('chlk.models.apps.AppModes');

NAMESPACE('chlk.templates.apps', function () {
    /** @class chlk.templates.apps.AppWrapperDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-wrapper-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppWrapperViewData)],
        'AppWrapperDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppModes, 'appMode',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppWrapperToolbarButton), 'buttons'
        ])
});