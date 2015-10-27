REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.apps.InstalledAppsViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppAttachModeDialogTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-attach-mode-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.InstalledAppsViewData)],
        'AppAttachModeDialogTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [
            [ria.templates.ModelPropertyBind('appUrlAppend')],
            String, 'url'
        ]);
});
