REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.RoomId');
REQUIRE('chlk.models.period.Period');
NAMESPACE('chlk.models.period', function () {
    "use strict";

    /** @class chlk.models.period.ClassPeriod*/
    CLASS(
        'ClassPeriod', [
            chlk.models.id.ClassPeriodId, 'id',

            chlk.models.period.Period, 'period',

            [ria.serialize.SerializeProperty('roomid')],
            chlk.models.id.RoomId, 'roomId',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId'
        ]);
});
