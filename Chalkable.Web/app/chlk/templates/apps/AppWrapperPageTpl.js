REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.apps.ExternalAttachAppViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppWrapperPageTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-wrapper-page.jade')],
        [ria.templates.ModelBind(chlk.models.apps.ExternalAttachAppViewData)],
        'AppWrapperPageTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'app',
            [ria.templates.ModelPropertyBind],
            String, 'url'
        ]);
});
