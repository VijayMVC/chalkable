NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PaginatedList */
    CLASS(
        'PaginatedList', [
            [[Function]],
            function $(itemClass) {
                this.itemClass = itemClass;
            },

            READONLY, Function, 'itemClass',

            ArrayOf(Object), 'items',

            Number, 'pageSize',
            Number, 'page',
            Number, 'count',

            VOID, function setItems(values) {
                VALIDATE_ARG('value', [ArrayOf(this.itemClass)], values);

                this.items = values;
            }
        ])
})