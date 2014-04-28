REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ClassAnnouncementType*/
    CLASS(
        'ClassAnnouncementType', EXTENDS(chlk.models.announcement.AnnouncementType), [
            Number, 'id',
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId'
        ]);
});
