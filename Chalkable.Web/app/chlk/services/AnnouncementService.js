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

            [[chlk.models.id.AnnouncementId, Object]],
            ria.async.Future, function uploadAttachment(announcementId, files) {
                return this.uploadFiles('AnnouncementAttachment/AddAttachment', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AttachmentId, Object]],
            ria.async.Future, function deleteAttachment(attachmentId) {
                return this.get('AnnouncementAttachment/DeleteAttachment', chlk.models.announcement.Announcement, {
                    announcementAttachmentId: attachmentId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, Number]],
            ria.async.Future, function addAnnouncement(classId_, announcementTypeId_) {
                return this.get('chalkable2/app/data/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    announcementTypeRef: announcementTypeId_
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId]],
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

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId]],
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

            [[chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function listLast(classId, announcementTypeId, schoolPersonId) {
                return this.get('chalkable2/app/data/listlast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    announcementTypeRef: announcementTypeId,
                    schoolPersonId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function listForMonth(classId_, date_) {
                return this.get('app/data/calendarMonth.json', ArrayOf(chlk.models.calendar.announcement.Day), {
                    //classId: classId_.valueOf(),
                    //date: date_.getDate()
                });
            },


            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function updateAnnouncement(id, name) {
                return this.post('chalkable2/app/data/edit.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function removeAnnouncement(id) {
                return this.post('Announcement/Delete.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },
            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function getAnnouncement(id) {
                return this.post('Announcement/Info.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            }
        ])
});