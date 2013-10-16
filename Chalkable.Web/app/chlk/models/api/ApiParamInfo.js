NAMESPACE('chlk.models.api', function () {
    "use strict";



    /** @class chlk.models.api.ApiParamInfo*/
    CLASS(
        'ApiParamInfo', [
            String, 'name',
            String, 'description',
            [ria.serialize.SerializeProperty('isnullable')],
            Boolean, 'optional'
            //"paramtype": 1
        ]);
});
