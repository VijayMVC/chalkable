REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradingService*/
    CLASS(
        'GradingService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getMapping() {
                return this.get('Grading/UpdateItem'/*'chalkable2/data/grading-style.json'*/, chlk.models.grading.Mapping, {});
            }
        ])
});