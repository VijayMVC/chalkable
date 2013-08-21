REQUIRE('chlk.models.bgtasks.BgTaskLog');
REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.models.id.BgTaskId');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLogListViewData*/
    CLASS(
        'BgTasksLogListViewData', [
            chlk.models.common.PaginatedList, 'items',
            chlk.models.id.BgTaskId, 'bgTaskId',


            [[chlk.models.id.BgTaskId, chlk.models.common.PaginatedList]],
            function $(bgTaskId, items){
                this.setBgTaskId(bgTaskId);
                this.setItems(items);
            }
        ]);
});
