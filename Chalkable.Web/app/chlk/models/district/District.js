REQUIRE('chlk.models.id.DistrictId');
NAMESPACE('chlk.models.district', function () {
    "use strict";

    /** @class chlk.models.district.District*/
    CLASS(
        'District', [
            chlk.models.id.DistrictId, 'id',
            String, 'name',
            [ria.serialize.SerializeProperty('sisurl')],
            String, 'sisUrl',
            [ria.serialize.SerializeProperty('dbname')],
            String, 'dbName',
            [ria.serialize.SerializeProperty('sisusername')],
            String, 'sisUserName',
            [ria.serialize.SerializeProperty('sispassword')],
            String, 'sisPassword',
            [ria.serialize.SerializeProperty('sissystemtype')],
            Number, 'sisSystemType'
        ]);
});
