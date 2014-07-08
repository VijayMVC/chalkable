REQUIRE('chlk.models.bgtasks.BgTask');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskList*/
    CLASS(
        'BgTaskList', [
            ArrayOf(chlk.models.bgtasks.BgTask), 'items'
        ]);
});
