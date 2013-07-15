REQUIRE('chlk.models.bgtasks.BgTaskLog');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLog*/
    CLASS(
        'BgTaskLog', [
            ArrayOf(chlk.models.bgtasks.BgTaskLog), 'items'
        ]);
});
