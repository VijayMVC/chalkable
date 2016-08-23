REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.apps.AppsForAttachViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachAppsDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-apps-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppsForAttachViewData)],
        'AttachAppsDialogTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps'

    ]);
});