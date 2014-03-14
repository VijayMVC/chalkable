REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.classes.ClassesForTopBar');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.templates.discipline.PaginatedListByDateModel*/

    CLASS('PaginatedListByDateModel', EXTENDS(chlk.models.common.PageWithClasses), [

        chlk.models.common.PaginatedList, 'items',
        chlk.models.common.ChlkDate, 'date',

        [[chlk.models.classes.ClassesForTopBar, chlk.models.common.PaginatedList, chlk.models.common.ChlkDate]],
        function $(topData_, items_, date_){
            BASE(topData_);
            if(items_)
                this.setItems(items_);
            if(date_)
                this.setDate(date_);
        }
    ]);
});