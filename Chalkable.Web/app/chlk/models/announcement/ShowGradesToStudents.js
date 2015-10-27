REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShowGradesToStudents*/
    CLASS(
        'ShowGradesToStudents', [
            chlk.models.id.AnnouncementId, 'announcementId',

            Boolean, 'showToStudents'
        ]);
});
