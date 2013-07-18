REQUIRE('chlk.models.announcement.AnnouncementType');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.class.ClassForTopBar*/
    CLASS(
        'ClassForTopBar', [
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.class.ClassId, 'classId',
            ArrayOf(Number), 'mask',
            [ria.serialize.SerializeProperty('typesbyclass')],
            ArrayOf(chlk.models.announcement.AnnouncementType), 'typesByClass'
        ]);
});
