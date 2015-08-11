REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.AnnouncementView');

REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeAttachmentId');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.id.AnnouncementQnAId');
REQUIRE('chlk.models.announcement.AnnouncementTitleViewData');
REQUIRE('chlk.models.announcement.AnnouncementAttributeType');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementService */
    CLASS(
        'AnnouncementService', EXTENDS(chlk.services.BaseService), [

            function $()
            {
                BASE();
                this.setCache({});
            },

            Object, 'cache',
            Number, 'importantCount',

            [[Number, chlk.models.id.ClassId, Boolean]],
            ria.async.Future, function getAnnouncements(pageIndex_, classId_, importantOnly_) {
                return this.get('Feed/List.json', ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), {
                    start: pageIndex_|0,
                    classId: classId_ ? classId_.valueOf() : null,
                    complete: importantOnly_ ? false : null
                });

            },

            [[Number, String, Boolean]],
            ria.async.Future, function getAnnouncementsForAdmin(pageIndex_, gradeLevels_, importantOnly_) {
                return this.get('Feed/DistrictAdminFeed.json', ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), {
                    gradeLevelIds : gradeLevels_,
                    start: pageIndex_|0,
                    complete: importantOnly_ ? false : null
                });

            },

            [[String, chlk.models.attachment.SortAttachmentType, Number, Number]],
            ria.async.Future, function getAttachments(filter_, sortType_, start_, count_){
                return this.getPaginatedList('Attachment/AttachmentsList.json', chlk.models.attachment.Attachment,{
                    filter: filter_,
                    sortType: sortType_ && sortType_.valueOf(),
                    start: start_,
                    count: count_
                });
            },

            [[chlk.models.id.AnnouncementId, Object, chlk.models.announcement.AnnouncementTypeEnum]],
                ria.async.Future, function uploadAttachment(announcementId, files, announcementType) {
                return this.uploadFiles('AnnouncementAttachment/UploadAnnouncementAttachment', files, chlk.models.announcement.FeedAnnouncementViewData, {
                        announcementId: announcementId.valueOf(),
                        announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId]],
            ria.async.Future, function addAttachment(announcementId, announcementType, attachmentId) {
                return this.post('AnnouncementAttachment/Add', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf(),
                    attachmentId: attachmentId.valueOf()
                });
            },


            [[String]],
            ria.async.Future, function getGradeComments(query_) {
                return this.get('Grading/GetGridComments', ArrayOf(String), {
                    schoolYearId: this.getContext().getSession().get(ChlkSessionConstants.CURRENT_SCHOOL_YEAR_ID, null).valueOf()
                }).then(function(data){
                    var comments = data || [];

                });
            },

            [[chlk.models.id.AnnouncementAttachmentId]],
            ria.async.Future, function startViewSession(announcementAttachmentId) {
                return this.get('AnnouncementAttachment/StartViewSession', String, {
                    announcementAttachmentId: announcementAttachmentId.valueOf(),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            },

            [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function cloneAttachment(attachmentId, announcementId, announcementType_) {
                //todo : remove getting announcementType  from session
                var annType = announcementType_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                return this.get('AnnouncementAttachment/CloneAttachment', chlk.models.announcement.AnnouncementView, {
                    originalAttachmentId: attachmentId.valueOf(),
                    announcementId: announcementId.valueOf(),
                    announcementType: annType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, Boolean]],
            ria.async.Future, function setShowGradesToStudents(announcementId, value) {
                return ria.async.DeferredData(true);
            },

            [[chlk.models.id.AnnouncementAttachmentId, Boolean, Number, Number]],
            String, function getAttachmentUri(announcementAttachmentId, needsDownload, width, height) {
                return this.getUrl('AnnouncementAttachment/DownloadAttachment', {
                    announcementAttachmentId: announcementAttachmentId.valueOf(),
                    needsDownload: needsDownload,
                    width: width,
                    height: height
                });
            },

            [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteAttachment(attachmentId, announcementId, announcementType) {
                return this.get('AnnouncementAttachment/DeleteAttachment.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementAttachmentId: attachmentId.valueOf(),
                    announcementId: announcementId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteApp(announcementAppId, announcementType) {
                return this.get('Application/RemoveFromAnnouncement.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementApplicationId: announcementAppId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate]],
            ria.async.Future, function addAnnouncement(classId_, classAnnouncementTypeId_, expiresDate_) {
                return this.get('Announcement/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    classAnnouncementTypeId: classAnnouncementTypeId_,
                    expiresDate: expiresDate_ ? expiresDate_.valueOf() : null
                });
            },
            
            [[chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function listLast(classId, classAnnouncementTypeId, schoolPersonId) {
                return this.get('Announcement/ListLast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    classAnnouncementTypeId: classAnnouncementTypeId,
                    personId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function editAnnouncement(id, announcementType) {
                return this.get('Announcement/Edit.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function deleteAnnouncement(id, announcementType) {
                return this.post('Announcement/Delete.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: id.valueOf(),
                    announcementType: announcementType && announcementType.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteDrafts(id, announcementType) {
                return this.post('Announcement/DeleteDrafts.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    personId: id.valueOf(),
                    announcementType: announcementType && announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function getAnnouncement(id, announcementType_) {
                return this.get('Announcement/Read.json', chlk.models.announcement.AnnouncementView, {
                    announcementId: id.valueOf(),
                    announcementType: announcementType_ && announcementType_.valueOf()
                })
                .then(function(announcement){
                  this.cache[announcement.getId().valueOf()] = announcement;
                  return announcement;
                }, this);
            },

            [[chlk.models.id.AnnouncementId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function checkItem(announcementId, complete_, type_) {
                this.setImportantCount(this.getImportantCount() + (complete_ ? 1 : -1));
                return this.post('Announcement/Complete', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    complete: complete_,
                    announcementType: type_ && type_.valueOf(),
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function askQuestion(announcementId, question) {
                return this.post('AnnouncementQnA/Ask', chlk.models.announcement.AnnouncementQnA, {
                    announcementId: announcementId.valueOf(),
                    question: question,
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                }).then(function(qna){
                    var result =this.cache[qna.getAnnouncementId().valueOf()];
                    result.getAnnouncementQnAs().push(qna);
                    return result;
                }, this);
            },


            [[chlk.models.id.AnnouncementQnAId, String, String]],
            ria.async.Future, function answerQuestion(announcementQnAId, question, answer) {
                return this.post('AnnouncementQnA/Answer', chlk.models.announcement.AnnouncementQnA, {
                    announcementQnAId: announcementQnAId.valueOf(),
                    question: question,
                    answer: answer,
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
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

            [[chlk.models.announcement.AnnouncementQnA]],
            Object, function editCacheAnnouncementQnAs_(qna){
                var result =this.cache[qna.getAnnouncementId().valueOf()];
                for (var i = 0; i < result.getAnnouncementQnAs().length; i++)
                    if (result.getAnnouncementQnAs()[i].getId() == qna.getId())
                    {
                        result.getAnnouncementQnAs()[i] = qna;
                        break;
                    }
                return result;
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementQnAId]],
            ria.async.Future, function deleteQnA(announcementId, announcementQnAId) {
                return this.post('AnnouncementQnA/Delete', ArrayOf(chlk.models.announcement.AnnouncementQnA), {
                    announcementQnAId: announcementQnAId.valueOf()
                }).then(function(qnas){
                    var result =this.cache[announcementId.valueOf()];
                    result.setAnnouncementQnAs(qnas);
                    return result;
                }, this);
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
            ria.async.Future, function addStandard(announcementId, standardId) {
                return this.get('Announcement/AddStandard.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    standardId: standardId.valueOf(),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
            ria.async.Future, function removeStandard(announcementId, standardId) {
                return this.get('Announcement/RemoveStandard.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    standardId: standardId.valueOf(),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            },

            [[ArrayOf(chlk.models.id.ClassId)]],
            ria.async.Future, function getClassAnnouncementTypes(classIds){
                return this.get('AnnouncementType/ListByClasses.json', ArrayOf(chlk.models.announcement.ClassAnnouncementType),{
                    classIds: this.arrayToCsv(classIds)
                });
            },

            [[chlk.models.id.ClassId, String, String, Number, Number, Boolean, Number]],
            ria.async.Future, function createAnnouncementTypes(classId, description_, name_, highScoresToDrop_, lowScoresToDrop_, isSystem_, percentage_){
                return this.get('AnnouncementType/Create.json', chlk.models.announcement.ClassAnnouncementType,{
                    classId: classId.valueOf(),
                    description: description_,
                    name: name_,
                    highScoresToDrop: highScoresToDrop_,
                    lowScoresToDrop: lowScoresToDrop_,
                    isSystem: isSystem_,
                    percentage: percentage_
                });
            },

            [[chlk.models.id.ClassId, String, String, Number, Number, Boolean, Number, Number]],
            ria.async.Future, function updateAnnouncementTypes(classId, description_, name_, highScoresToDrop_, lowScoresToDrop_, isSystem_, percentage_, classAnnouncementTypeId_){
                return this.get('AnnouncementType/Update.json', chlk.models.announcement.ClassAnnouncementType,{
                    classAnnouncementTypeId: classAnnouncementTypeId_,
                    classId: classId.valueOf(),
                    description: description_,
                    name: name_,
                    highScoresToDrop: highScoresToDrop_,
                    lowScoresToDrop: lowScoresToDrop_,
                    isSystem: isSystem_,
                    percentage: percentage_
                });
            },

            [[Array]],
            ria.async.Future, function deleteAnnouncementTypes(ids){
                return this.get('AnnouncementType/Delete.json', Boolean,{
                    classAnnouncementTypeIds: this.arrayToCsv(ids),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            },




        ])
});