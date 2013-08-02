REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassForWeekMask*/
    CLASS(
        'ClassForWeekMask', [
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',
            ArrayOf(Number), 'mask',
            ArrayOf(chlk.models.announcement.AnnouncementType), 'typesByClass'
        ]);
});
