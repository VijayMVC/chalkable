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
REQUIRE('chlk.models.id.AnnouncementAttachmentId');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.id.AnnouncementQnAId');
REQUIRE('chlk.models.announcement.AnnouncementTitleViewData');


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

            [[chlk.models.id.AnnouncementId, Object]],
            ria.async.Future, function uploadAttachment(announcementId, files) {
                return this.uploadFiles('AnnouncementAttachment/AddAttachment', files, chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementAttachmentId]],
            ria.async.Future, function startViewSession(announcementAttachmentId) {
                return this.get('AnnouncementAttachment/StartViewSession', String, {
                    announcementAttachmentId: announcementAttachmentId.valueOf()
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

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function editTitle(announcementId, title) {
                return this.get('Announcement/EditTitle.json', Boolean, {
                    announcementId: announcementId.valueOf(),
                    title: title
                });
            },

            [[String, chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function existsTitle(title, classId, expiresDate) {
                return this.get('Announcement/Exists.json', Boolean, {
                    title: title,
                    classId: classId.valueOf(),
                    expiresDate: expiresDate && expiresDate.toStandardFormat()
                });
            },

            [[chlk.models.id.AttachmentId, Object]],
            ria.async.Future, function deleteAttachment(attachmentId) {
                return this.get('AnnouncementAttachment/DeleteAttachment.json', chlk.models.announcement.Announcement, {
                    announcementAttachmentId: attachmentId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementApplicationId]],
            ria.async.Future, function deleteApp(announcementAppId) {
                return this.get('Application/RemoveFromAnnouncement.json', chlk.models.announcement.Announcement, {
                    announcementApplicationId: announcementAppId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, Number]],
            ria.async.Future, function addAnnouncement(classId_, classAnnouncementTypeId_) {
                return this.get('Announcement/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    classAnnouncementTypeId: classAnnouncementTypeId_
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String
                , chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId
                , Number, Number, Number, Boolean, Boolean
            ]],
            ria.async.Future, function saveAnnouncement(id, classId_, classAnnouncementTypeId_, title_, content_
                , expiresdate_, attachments_, applications_, markingPeriodId_, maxScore_, weightAddition_, weighMultiplier_
                , hideFromStudent_, canDropStudentScore_) {
                return this.get('Announcement/SaveAnnouncement.json', chlk.models.announcement.Announcement, {
                    announcementId:id.valueOf(),
                    classAnnouncementTypeId:classAnnouncementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId:markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    maxscore: maxScore_,
                    weightaddition: weightAddition_,
                    weightmultiplier: weighMultiplier_,
                    hidefromstudents: hideFromStudent_ || false,
                    candropstudentscore: canDropStudentScore_ || false
                });
            },

            [[chlk.models.id.AnnouncementId, String,String, String, chlk.models.common.ChlkDate,  String]],
            ria.async.Future, function saveAdminAnnouncement(id, recipients,title_, content_, expiresdate_, attachments_) {
                return this.get('Announcement/SaveForAdmin.json', chlk.models.announcement.AnnouncementTitleViewData, {
                    announcementId:id.valueOf(),
                    content: content_,
                    attachments: attachments_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    annRecipients: recipients
                });
            },

            [[chlk.models.id.AnnouncementId, String,String, String, chlk.models.common.ChlkDate,  String]],
            ria.async.Future, function submitAdminAnnouncement(id, recipients,title_, content_, expiresdate_, attachments_) {
                return this.post('Announcement/SubmitForAdmin.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId:id.valueOf(),
                    content: content_,
                    attachments: attachments_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    annRecipients: recipients
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String
                , chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId
                , Number, Number, Number, Boolean, Boolean
            ]],
            ria.async.Future, function submitAnnouncement(id, classId_, announcementTypeId_, title_, content_
                , expiresdate_, attachments_, applications_, markingPeriodId_, maxScore_, weightAddition_, weighMultiplier_
                , hideFromStudent_, canDropStudentScore_) {
                return this.post('Announcement/SubmitAnnouncement.json', Boolean, {
                    announcementid:id.valueOf(),
                    classannouncementtypeid:announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingperiodid: markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    maxscore: maxScore_,
                    weightaddition: weightAddition_,
                    weightmultiplier: weighMultiplier_,
                    hidefromstudents: hideFromStudent_ || false,
                    candropstudentscore: canDropStudentScore_ || false
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

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function editAnnouncement(id) {
                return this.get('Announcement/Edit.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function duplicateAnnouncement(id, classIds) {
                return this.get('Announcement/DuplicateAnnouncement.json', Boolean, {
                    announcementId: id.valueOf(),
                    classIds: classIds
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function deleteAnnouncement(id) {
                return this.post('Announcement/Delete.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function dropAnnouncement(id) {
                return this.post('Announcement/DropAnnouncement.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function unDropAnnouncement(id) {
                return this.post('Announcement/UndropAnnouncement.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function deleteDrafts(id) {
                return this.post('Announcement/DeleteDrafts.json', chlk.models.announcement.Announcement, {
                    personId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function getAnnouncement(id) {
                return this.get('Announcement/Read.json', chlk.models.announcement.AnnouncementView, {
                    announcementId: id.valueOf()
                })
                .then(function(announcement){
                  this.cache[announcement.getId().valueOf()] = announcement;
                  return announcement;
                }, this);
            },

            [[chlk.models.id.AnnouncementId, Boolean]],
            ria.async.Future, function checkItem(announcementId, complete_) {
                this.setImportantCount(this.getImportantCount() + (complete_ ? 1 : -1));
                return this.post('Announcement/Complete', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf(),
                    complete: complete_
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function makeVisible(announcementId) {
                return this.post('Announcement/MakeVisible', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function askQuestion(announcementId, question) {
                return this.post('AnnouncementQnA/Ask', chlk.models.announcement.AnnouncementQnA, {
                    announcementId: announcementId.valueOf(),
                    question: question
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
                    answer: answer
                }).then(function(qna){
                    var result =this.cache[qna.getAnnouncementId().valueOf()];
                    for (var i = 0; i < result.getAnnouncementQnAs().length; i++)
                        if (result.getAnnouncementQnAs()[i].getId() == qna.getId())
                        {
                            result.getAnnouncementQnAs()[i] = qna;
                            break;
                        }
                    return result;
                }, this);
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
                return this.get('Announcement/AddStandard.json', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf(),
                    standardId: standardId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
            ria.async.Future, function removeStandard(announcementId, standardId) {
                return this.get('Announcement/RemoveStandard.json', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf(),
                    standardId: standardId.valueOf()
                });
            },


        ])
});