REQUIRE('chlk.models.apps.AppScreenshots');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppScreenshots*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-screenshots.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppScreenshots)],
        'AppScreenshots', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppPicture), 'items',
            [ria.templates.ModelPropertyBind],
            String, 'ids',
            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly'
        ])
});