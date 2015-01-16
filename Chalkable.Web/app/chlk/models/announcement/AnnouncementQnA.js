REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
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

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AnnouncementQnA*/
    CLASS(
        UNSAFE, FINAL, 'AnnouncementQnA', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementQnAId);
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.question = SJX.fromDeserializable(raw.question, chlk.models.announcement.AnnouncementMessage);
                this.answer = SJX.fromDeserializable(raw.answer, chlk.models.announcement.AnnouncementMessage);
                this.owner = SJX.fromValue(raw.isowner, Boolean);
                this.state = SJX.fromValue(raw.state, chlk.models.announcement.QnAState);
            },

            chlk.models.id.AnnouncementQnAId, 'id',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.announcement.AnnouncementMessage, 'question',
            chlk.models.announcement.AnnouncementMessage, 'answer',
            Boolean, 'owner',
            chlk.models.announcement.QnAState, 'state'
        ]);
});
