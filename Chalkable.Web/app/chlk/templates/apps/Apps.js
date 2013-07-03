REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/Apps.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Apps', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.apps.Application), 'items',
            [ria.templates.ModelBind],
            Number, 'pageindex',
            [ria.templates.ModelBind],
            Number, 'pagesize',
            [ria.templates.ModelBind],
            Number, 'totalcount',
            [ria.templates.ModelBind],
            Number, 'totalpages',
            [ria.templates.ModelBind],
            Boolean, 'hasnextpage',
            [ria.templates.ModelBind],
            Boolean, 'haspreviouspage'
        ])
});