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

            Number, 'pageindex',
            Number, 'pagesize',
            Number, 'totalcount',
            Number, 'totalpages',
            Boolean, 'hasnextpage',
            Boolean, 'haspreviouspage',

            VOID, function setItems(values) {
                VALIDATE_ARG('value', [ArrayOf(this.itemClass)], values);

                this.items = values;
            }
        ])
})