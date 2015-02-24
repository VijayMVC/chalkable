REQUIRE('chlk.models.api.ApiParamType');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiParamInfo*/
    CLASS(
        'ApiParamInfo', [
            String, 'name',
            String, 'description',

            [ria.serialize.SerializeProperty('isnullable')],
            Boolean, 'optional',

            [ria.serialize.SerializeProperty('paramtype')],
            chlk.models.api.ApiParamType, 'paramType'
        ]);
});
