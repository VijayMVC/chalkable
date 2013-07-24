REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.class.ClassesForTopBar');

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

            [[chlk.models.class.ClassId, Number]],
            ria.async.Future, function addAnnouncement(classId_, announcementTypeId_) {
                return this.get('app/data/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_.valueOf(),
                    announcementTypeRef: announcementTypeId_
                });
            },

            [[chlk.models.announcement.AnnouncementId, chlk.models.class.ClassId, Number, String, String, String, String, String, chlk.models.class.MarkingPeriodId]],
            ria.async.Future, function saveAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get('Announcement/Save.json', chlk.models.announcement.AnnouncementForm, {
                    id:id.valueOf(),
                    announcementTypeRef:announcementTypeId_,
                    classId: classId_.valueOf(),
                    markingPeriodId:markingPeriodId_,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            },

            [[chlk.models.announcement.AnnouncementId, chlk.models.class.ClassId, Number, String, String, String, String, String, chlk.models.class.MarkingPeriodId]],
            ria.async.Future, function submitAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get('Announcement/SubmitAnnouncement.json', chlk.models.announcement.AnnouncementForm, {
                    id:id.valueOf(),
                    announcementTypeRef:announcementTypeId_,
                    classId: classId_.valueOf(),
                    markingPeriodId:markingPeriodId_.valueOf(),
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            },

            [[chlk.models.class.ClassId, Number, chlk.models.people.SchoolPersonId]],
            ria.async.Future, function listLast(classId, announcementTypeId, schoolPersonId) {
                return this.get('app/data/listlast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    announcementTypeRef: announcementTypeId,
                    schoolPersonId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.announcement.AnnouncementId, String]],
            ria.async.Future, function updateAnnouncement(id, name) {
                return this.post('Announcement/Edit.json', chlk.models.announcement.Announcement, {
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