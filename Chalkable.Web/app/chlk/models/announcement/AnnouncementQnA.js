REQUIRE('chlk.models.id.AnnouncementQnAId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.announcement.AnnouncementMessage');


NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.QnAState*/
    ENUM('QnAState', {
        ASKED: 0,
        ANSWERED: 1,
        UNANSWERED: 2
    });

    /** @class chlk.models.announcement.AnnouncementQnA*/
    CLASS(
        'AnnouncementQnA', [
            chlk.models.id.AnnouncementQnAId, 'id',

            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            chlk.models.announcement.AnnouncementMessage, 'question',

            chlk.models.announcement.AnnouncementMessage, 'answer',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'owner',

            chlk.models.announcement.QnAState, 'state'
        ]);
});
