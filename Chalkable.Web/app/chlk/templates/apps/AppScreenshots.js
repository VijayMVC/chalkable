REQUIRE('chlk.models.apps.AppScreenShots');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppScreenshots*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-screenshots.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppScreenShots)],
        'AppScreenshots', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppPicture), 'items',
            [ria.templates.ModelPropertyBind],
            String, 'ids',
            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly',
            [ria.templates.ModelPropertyBind],
            String, 'templateDownloadLink'
        ])
});