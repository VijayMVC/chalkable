NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PaginatedList */
    CLASS(
        'PaginatedList', [
            function $(itemClass) {
                VALIDATE_ARG('itemClass', [Function, ria.__API.SpecifyDescriptor], itemClass);
                BASE();
                this.itemClass = itemClass;
            },

            READONLY, Object, 'itemClass',

            ArrayOf(Object), 'items',
            Number, 'pageIndex',
            Number, 'pageSize',
            Number, 'totalCount',
            Number, 'actualCount',
            Number, 'totalPages',
            Boolean, 'hasNextPage',
            Boolean, 'hasPreviousPage',
            String, 'filter',

            VOID, function setItems(values) {
                VALIDATE_ARG('value', [ArrayOf(this.itemClass)], values);
                this.items = values;
            }
        ])
});