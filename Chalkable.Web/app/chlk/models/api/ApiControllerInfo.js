REQUIRE('chlk.models.api.ApiMethodInfo');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiControllerInfo*/
    CLASS(
        'ApiControllerInfo', [
            [ria.serialize.SerializeProperty('controllername')],
            String, 'name',

            ArrayOf(chlk.models.api.ApiMethodInfo), 'methods'
        ]);
});
