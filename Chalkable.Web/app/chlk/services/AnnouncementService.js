REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.class.ClassesForTopBar');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementService */
    CLASS(
        'AnnouncementService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getAnnouncements(pageIndex_) {
                return this.getPaginatedList('chalkable2/app/data/feed.json', chlk.models.announcement.Announcement, {
                    start: pageIndex_|0
                });
            },

            [[chlk.models.announcement.AnnouncementId, Object]],
            ria.async.Future, function uploadAttachment(announcementId, files) {
                return this.uploadFiles('AnnouncementAttachment/AddAttachment', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.attachment.AttachmentId, Object]],
            ria.async.Future, function deleteAttachment(attachmentId) {
                return this.get('AnnouncementAttachment/DeleteAttachment', chlk.models.announcement.Announcement, {
                    announcementAttachmentId: attachmentId.valueOf()
                });
            },

            [[chlk.models.class.ClassId, Number]],
            ria.async.Future, function addAnnouncement(classId_, announcementTypeId_) {
                return this.get('chalkable2/app/data/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    announcementTypeRef: announcementTypeId_
                });
            },

            [[chlk.models.announcement.AnnouncementId, chlk.models.class.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.class.MarkingPeriodId]],
            ria.async.Future, function saveAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get('Announcement/Save.json', chlk.models.announcement.AnnouncementForm, {
                    id:id.valueOf(),
                    announcementTypeRef:announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId:markingPeriodId_,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            },

            [[chlk.models.announcement.AnnouncementId, chlk.models.class.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.class.MarkingPeriodId]],
            ria.async.Future, function submitAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get('Announcement/SubmitAnnouncement.json', chlk.models.announcement.AnnouncementForm, {
                    id:id.valueOf(),
                    announcementTypeRef:announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId: markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            },

            [[chlk.models.class.ClassId, Number, chlk.models.people.SchoolPersonId]],
            ria.async.Future, function listLast(classId, announcementTypeId, schoolPersonId) {
                return this.get('chalkable2/app/data/listlast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    announcementTypeRef: announcementTypeId,
                    schoolPersonId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.announcement.AnnouncementId, String]],
            ria.async.Future, function updateAnnouncement(id, name) {
                return this.post('chalkable2/app/data/edit.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.announcement.AnnouncementId]],
            ria.async.Future, function removeAnnouncement(id) {
                return this.post('Announcement/Delete.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },
            [[chlk.models.announcement.AnnouncementId]],
            ria.async.Future, function getAnnouncement(id) {
                return this.post('Announcement/Info.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            }
        ])
});