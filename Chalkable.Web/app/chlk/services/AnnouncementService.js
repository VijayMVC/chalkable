REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');

REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.id.AnnouncementQnAId');


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

            [[Number, chlk.models.id.ClassId]],
            ria.async.Future, function getAnnouncements(pageIndex_, classId_) {
                return this.getPaginatedList('Feed/List.json', chlk.models.announcement.Announcement, {
                    start: pageIndex_|0,
                    classId: classId_ ? classId_.valueOf() : null
                });
            },

            [[chlk.models.id.AnnouncementId, Object]],
            ria.async.Future, function uploadAttachment(announcementId, files) {
                return this.uploadFiles('AnnouncementAttachment/AddAttachment', files, chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementAttachmentId, Boolean, Number, Number]],
            String, function getAttachmentUri(announcementAttachmentId, needsDownload, width, heigh) {
                return this.getUrl('AnnouncementAttachment/DownloadAttachment', {
                    announcementAttachmentId: announcementAttachmentId.valueOf(),
                    needsDownload: needsDownload,
                    width: width,
                    heigh: heigh
                });
            },

            [[chlk.models.id.AttachmentId, Object]],
            ria.async.Future, function deleteAttachment(attachmentId) {
                return this.get('AnnouncementAttachment/DeleteAttachment.json', chlk.models.announcement.Announcement, {
                    announcementAttachmentId: attachmentId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, Number]],
            ria.async.Future, function addAnnouncement(classId_, announcementTypeId_) {
                return this.get('Announcement/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    announcementTypeRef: announcementTypeId_
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId]],
            ria.async.Future, function saveAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get('Announcement/SaveAnnouncement.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId:id.valueOf(),
                    announcementTypeId:announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId:markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            },

            [[chlk.models.id.AnnouncementId, Array,String, String, chlk.models.common.ChlkDate,  String]],
            ria.async.Future, function saveAdminAnnouncement(id, recipients,subject_, content_, expiresdate_, attachments_) {
                return this.get('Announcement/SaveForAdmin.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId:id.valueOf(),
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    expiresdate: expiresdate_,
                    annRecipients: recipients
                });
            },

            [[chlk.models.id.AnnouncementId, Array,String, String, chlk.models.common.ChlkDate,  String]],
            ria.async.Future, function submitAdminAnnouncement(id, recipients,subject_, content_, expiresdate_, attachments_) {
                return this.get('Announcement/SubmitForAdmin.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId:id.valueOf(),
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    expiresdate: expiresdate_,
                    annRecipients: recipients
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId]],
            ria.async.Future, function submitAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get('Announcement/SubmitAnnouncement.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId:id.valueOf(),
                    announcementTypeId:announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId: markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            },




            [[chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function listLast(classId, announcementTypeId, schoolPersonId) {
                return this.get('Announcement/ListLast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    announcementTypeId: announcementTypeId,
                    personId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function editAnnouncement(id) {
                return this.get('Announcement/Edit.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function deleteAnnouncement(id) {
                return this.post('Announcement/Delete.json', chlk.models.announcement.Announcement, {
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
                return this.get('Announcement/Read.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                })
                .then(function(announcement){
                      this.cache[announcement.getId().valueOf()] = announcement;
                      return announcement;
                    }.bind(this)
                );
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function star(announcementId) {
                return this.post('Announcement/Star', chlk.models.announcement.Announcement, {
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
                    }.bind(this)
                );
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
                }.bind(this)
                );
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementQnAId]],
            ria.async.Future, function deleteQnA(announcementId, announcementQnAId) {
                return this.post('AnnouncementQnA/Delete', ArrayOf(chlk.models.announcement.AnnouncementQnA), {
                    announcementQnAId: announcementQnAId.valueOf()
                }).then(function(qnas){
                        var result =this.cache[announcementId.valueOf()];
                        result.setAnnouncementQnAs(qnas);
                        return result;
                    }.bind(this)
                );
            }
        ])
});