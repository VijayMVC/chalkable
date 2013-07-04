REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/Apps.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Apps', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.apps.Application), 'items'
        ])
});