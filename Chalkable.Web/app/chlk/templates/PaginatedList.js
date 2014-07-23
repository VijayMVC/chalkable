REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.PaginatedList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/paginator.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'PaginatedList', EXTENDS(chlk.templates.ChlkTemplate), [
            /*[ria.templates.ModelPropertyBind],
            ArrayOf(Object), 'items',*/
            [ria.templates.ModelPropertyBind],
            Number, 'pageIndex',
            [ria.templates.ModelPropertyBind],
            Number, 'pageSize',
            [ria.templates.ModelPropertyBind],
            Number, 'totalCount',
            [ria.templates.ModelPropertyBind],
            Number, 'totalPages',
            [ria.templates.ModelPropertyBind],
            Boolean, 'hasNextPage',
            [ria.templates.ModelPropertyBind],
            Boolean, 'hasPreviousPage'
        ])
});