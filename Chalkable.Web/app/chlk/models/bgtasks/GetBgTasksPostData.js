REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";

    /** @class chlk.models.bgtasks.GetBgTasksPostData*/
    CLASS(
        'GetBgTasksPostData', [

            Number, 'start',
            Number, 'state',
            Number, 'type',
            [ria.serialize.SerializeProperty('districtid')],
            chlk.models.id.DistrictId, 'districtId',

            [ria.serialize.SerializeProperty('isalldistricts')],
            Boolean, 'allDistricts',
        ]);
});
