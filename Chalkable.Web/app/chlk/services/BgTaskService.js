REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.bgtasks.BgTask');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradeLevelService*/
    CLASS(
        'BgTaskService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getTasks(pageIndex_) {
                return this.getPaginatedList('/app/data/bgtasks.json', chlk.models.bgtasks.BgTask, pageIndex_);
            }
        ])
});