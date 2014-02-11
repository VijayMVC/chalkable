REQUIRE('chlk.models.announcement.ShortStudentAnnouncementsViewData');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortAnnouncementViewData*/
    CLASS(
        'ShortAnnouncementViewData', [
            [ria.serialize.SerializeProperty('announcementtypename')],
            String, 'announcementTypeName',

            Number, 'order',

            String, 'title',

            [ria.serialize.SerializeProperty('expiresdate')],
            chlk.models.common.ChlkDate, 'expiresDate',

            Number, 'avg',

            chlk.models.id.AnnouncementId, 'id',

            Boolean, 'dropped',

            [ria.serialize.SerializeProperty('studentannouncements')],
            chlk.models.announcement.ShortStudentAnnouncementsViewData, 'studentAnnouncements',

            [ria.serialize.SerializeProperty('maxscore')],
            Number, 'maxScore'
        ]);
});
