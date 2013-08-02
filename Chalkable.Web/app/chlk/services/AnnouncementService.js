REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');

REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.class.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementService */
    CLASS(
        'AnnouncementService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getAnnouncements(pageIndex_) {
                return this.getPaginatedList('Feed/List.json', chlk.models.announcement.Announcement, {
                    start: pageIndex_|0
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
                    announcementTypeRef: announcementTypeId,
                    schoolPersonId: schoolPersonId.valueOf()
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
                });
            }
        ])
});