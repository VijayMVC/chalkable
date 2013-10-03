REQUIRE('chlk.models.apps.AppWrapperViewData');

NAMESPACE('chlk.templates.apps', function () {
    /** @class chlk.templates.apps.AppWrapperDialogToolbarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppWrapperDialogToolbar.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppWrapperViewData)],
        'AppWrapperDialogToolbarTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppAttachment, 'app',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppWrapperToolbarButton), 'toolbarButtons'
        ])
});