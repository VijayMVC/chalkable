REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.Apps*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/apps.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Apps', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'items'
        ])
});