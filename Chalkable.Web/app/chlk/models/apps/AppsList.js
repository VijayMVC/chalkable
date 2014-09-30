REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppsList*/
    CLASS(
        'AppsList', [
            ArrayOf(chlk.models.apps.Application), 'items'
        ]);
});