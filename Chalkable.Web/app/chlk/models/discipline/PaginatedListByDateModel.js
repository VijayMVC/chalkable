REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.templates.discipline.PaginatedListByDateModel*/

    CLASS('PaginatedListByDateModel', [

        chlk.models.common.PaginatedList, 'items',
        chlk.models.common.ChlkDate, 'date',

        [[chlk.models.common.PaginatedList, chlk.models.common.ChlkDate]],
        function $(items_, date_){
            BASE();
            if(items_)
                this.setItems(items_);
            if(date_)
                this.setDate(date_);
        }
    ]);
});