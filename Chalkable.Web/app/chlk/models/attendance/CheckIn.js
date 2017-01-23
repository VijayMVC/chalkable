REQUIRE('chlk.models.common.ChlkTime');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.CheckIn*/
    CLASS(
        'CheckIn', [

            [ria.serialize.SerializeProperty('checkintime')],
            chlk.models.common.ChlkTime, 'checkInTime',

            [ria.serialize.SerializeProperty('ischeckin')],
            Boolean, 'checkIn'

        ]);
});
