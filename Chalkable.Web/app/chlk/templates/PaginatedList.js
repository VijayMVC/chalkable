REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.PaginatedList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/paginator.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'PaginatedList', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            ArrayOf(Object), 'items',
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