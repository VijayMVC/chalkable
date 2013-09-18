REQUIRE('chlk.models.calendar.announcement.MonthItem');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.common.PaginatedList');


NAMESPACE('chlk.models.feed', function () {
    "use strict";

    /** @class chlk.models.feed.Feed*/
    CLASS(
        'Feed', EXTENDS(chlk.models.common.PaginatedList), [

            [[chlk.models.common.PaginatedList]],
            function $(list) {
                BASE(list.getItemClass());

                this.setItems(list.getItems());
                this.setPageIndex(list.getPageIndex());
                this.setPageSize(list.getPageSize());
                this.setTotalCount(list.getTotalCount());
                this.setTotalPages(list.getTotalPages());
                this.setHasNextPage(list.isHasNextPage());
                this.setHasPreviousPage(list.isHasPreviousPage());
            },

            chlk.models.classes.ClassesForTopBar, 'topData',
            Boolean, 'starredOnly',
            Number, 'importantCount'
        ]);
});


/*
 READONLY, Function, 'itemClass',

 ArrayOf(Object), 'items',
 Number, 'pageIndex',
 Number, 'pageSize',
 Number, 'totalCount',
 Number, 'totalPages',
 Boolean, 'hasNextPage',
 Boolean, 'hasPreviousPage',
    */