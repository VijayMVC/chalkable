NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PaginatedList */
    CLASS(
        'PaginatedList', [
            [[Function]],
            function $(itemClass) {
                BASE();
                this.itemClass = itemClass;
            },

            READONLY, Function, 'itemClass',

            ArrayOf(Object), 'items',
            Number, 'pageIndex',
            Number, 'pageSize',
            Number, 'totalCount',
            Number, 'totalPages',
            Boolean, 'hasNextPage',
            Boolean, 'hasPreviousPage',

            VOID, function setItems(values) {
                VALIDATE_ARG('value', [ArrayOf(this.itemClass)], values);
                this.items = values;
            }
        ])
})