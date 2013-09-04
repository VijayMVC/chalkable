REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachAppDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.InstalledAppsViewData)],
        'AttachAppDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'teacherId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps'

        ])
});