REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.apps.AppWrapperViewData');
REQUIRE('chlk.models.apps.AppModes');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.templates.apps', function () {
    /** @class chlk.templates.apps.AppWrapperDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-wrapper-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppWrapperViewData)],
        'AppWrapperDialogTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppModes, 'appMode',
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppAttachment, 'app',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppWrapperToolbarButton), 'toolbarButtons'

        ])
});