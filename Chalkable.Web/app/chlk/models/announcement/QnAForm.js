REQUIRE('chlk.models.id.AnnouncementQnAId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";
    /** @class chlk.models.announcement.QnAForm*/
    CLASS(
        'QnAForm', [
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.AnnouncementQnAId, 'id',
            String, 'question',
            String, 'answer',
            String, 'updateQuestionType',
            String, 'updateAnswerType'
        ]);
});

