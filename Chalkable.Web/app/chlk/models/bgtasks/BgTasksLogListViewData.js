REQUIRE('chlk.models.bgtasks.BgTaskLog');
REQUIRE('chlk.models.bgtasks.BgTask');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLogListViewData*/
    CLASS(
        'BgTasksLogListViewData', [
            chlk.models.common.PaginatedList, 'items',
            chlk.models.bgtasks.BgTaskId, 'bgTaskId',


            [[chlk.models.bgtasks.BgTaskId, chlk.models.common.PaginatedList]],
            function $(bgTaskId, items){
                this.setBgTaskId(bgTaskId);
                this.setItems(items);
            }
        ]);
});
