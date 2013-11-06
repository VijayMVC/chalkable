REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassForWeekMask*/
    CLASS(
        'ClassForWeekMask', [
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',
            ArrayOf(Number), 'mask',
            ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'typesByClass'
        ]);
});
