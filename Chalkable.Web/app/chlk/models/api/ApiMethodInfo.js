REQUIRE('chlk.models.api.ApiParamInfo');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiMethodInfo*/
    CLASS(
        'ApiMethodInfo', [
            String, 'name',

            [ria.serialize.SerializeProperty('method')],
            String, 'methodType',
            String, 'description',
            String, 'response',
            ArrayOf(chlk.models.api.ApiParamInfo), 'params'
        ]);
});
