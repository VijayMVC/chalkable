REQUIRE('chlk.models.announcement.AnnouncementType');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.class.ClassForWeekMask*/
    CLASS(
        'ClassForWeekMask', [
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.class.ClassId, 'classId',
            ArrayOf(Number), 'mask',
            ArrayOf(chlk.models.announcement.AnnouncementType), 'typesByClass'
        ]);
});
