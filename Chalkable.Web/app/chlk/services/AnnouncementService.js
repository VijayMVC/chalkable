REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.Announcement');

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

            [[chlk.models.announcement.AnnouncementTypeId, chlk.models.announcement.ClassId]],
            ria.async.Future, function addAnnouncement(typeId, classId) {
                return this.post('app/data/create.json', chlk.models.announcement.Announcement, {
                    announcementTypeId: typeId,
                    classId: classId
                });
            },

            [[chlk.models.announcement.AnnouncementId, String]],
            ria.async.Future, function updateAnnouncement(id, name) {
                return this.post('Announcement/Update.json', chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.announcement.AnnouncementId, String]],
            ria.async.Future, function saveAnnouncement(id_, name) {
                if (id_ && id_.valueOf()) return this.updateAnnouncement(id_);
                return this.addAnnouncement(name);
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