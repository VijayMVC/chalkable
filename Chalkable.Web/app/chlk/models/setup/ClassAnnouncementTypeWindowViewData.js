REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.ClassAnnouncementTypeWindowViewData*/
    CLASS(
        'ClassAnnouncementTypeWindowViewData', EXTENDS(chlk.models.announcement.ClassAnnouncementType), [
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId'
        ]);
});
