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
            Number, 'pageIndex',
            [ria.templates.ModelBind],
            Number, 'pageSize',
            [ria.templates.ModelBind],
            Number, 'totalCount',
            [ria.templates.ModelBind],
            Number, 'totalPages',
            [ria.templates.ModelBind],
            Boolean, 'hasNextPage',
            [ria.templates.ModelBind],
            Boolean, 'hasPreviousPage'
        ])
});