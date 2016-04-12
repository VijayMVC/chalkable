REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.announcement.AnnouncementQnA');
//REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
//REQUIRE('chlk.models.announcement.AnnouncementView');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementQnAService */
    CLASS(
        'AnnouncementQnAService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.AnnouncementId, String, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function askQuestion(announcementId, question, announcementType_) {
                announcementType_ = announcementType_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                return this.post('AnnouncementQnA/Ask', chlk.models.announcement.AnnouncementQnA, {
                    announcementId: announcementId.valueOf(),
                    question: question,
                    announcementType: announcementType_.valueOf()
                }).then(function(qna){
                    var result =this.getAnnouncement_(qna.getAnnouncementId());
                    result.getAnnouncementQnAs().push(qna);
                    return result;
                }, this);
            },


            [[chlk.models.id.AnnouncementQnAId, String, String, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function answerQuestion(announcementQnAId, question, answer, announcementType_) {
                announcementType_ = announcementType_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                return this.post('AnnouncementQnA/Answer', chlk.models.announcement.AnnouncementQnA, {
                    announcementQnAId: announcementQnAId.valueOf(),
                    question: question,
                    answer: answer,
                    announcementType: announcementType_.valueOf()
                }).then(function(qna){
                    return this.editCacheAnnouncementQnAs_(qna);
                }, this);
            },


            [[chlk.models.id.AnnouncementQnAId, String]],
            ria.async.Future, function editQuestion(announcementQnAId, question) {
                return this.post('AnnouncementQnA/EditQuestion', chlk.models.announcement.AnnouncementQnA, {
                    announcementQnAId: announcementQnAId.valueOf(),
                    question: question
                }).then(function(qna){
                    return this.editCacheAnnouncementQnAs_(qna);
                }, this);
            },

            [[chlk.models.id.AnnouncementQnAId, String]],
            ria.async.Future, function editAnswer(announcementQnAId, answer) {
                return this.post('AnnouncementQnA/EditAnswer', chlk.models.announcement.AnnouncementQnA, {
                    announcementQnAId: announcementQnAId.valueOf(),
                    answer: answer
                }).then(function(qna){
                    return this.editCacheAnnouncementQnAs_(qna);
                }, this);
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementQnAId]],
            ria.async.Future, function deleteQnA(announcementId, announcementQnAId) {
                return this.post('AnnouncementQnA/Delete', ArrayOf(chlk.models.announcement.AnnouncementQnA), {
                    announcementQnAId: announcementQnAId.valueOf()
                }).then(function(qnas){
                    var result =this.getAnnouncement_(announcementId);
                    result.setAnnouncementQnAs(qnas);
                    return result;
                }, this);
            },

            [[chlk.models.announcement.AnnouncementQnA]],
            Object, function editCacheAnnouncementQnAs_(qna){
                var result =this.getAnnouncement_(qna.getAnnouncementId());
                for (var i = 0; i < result.getAnnouncementQnAs().length; i++)
                    if (result.getAnnouncementQnAs()[i].getId() == qna.getId())
                    {
                        result.getAnnouncementQnAs()[i] = qna;
                        break;
                    }
                return result;
            },

            [[chlk.models.id.AnnouncementId]],
            chlk.models.announcement.AnnouncementView, function getAnnouncement_(id){
                return this.getContext().getService(chlk.services.AnnouncementService).getAnnouncementSync(id);
            }
        ]);
});
