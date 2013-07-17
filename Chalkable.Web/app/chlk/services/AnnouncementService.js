REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.feed.Announcement');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementService */
    CLASS(
        'AnnouncementService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getAnnouncements(pageIndex_) {
                return this.getPaginatedList('Feed/List.json', chlk.models.feed.Announcement, {
                    start: pageIndex_|0
                });
            },

            [[String]],
            ria.async.Future, function addAnnouncement(name) {
                return this.post('Announcement/Create.json', chlk.models.feed.Announcement, {
                    name: name
                });
            },

            [[chlk.models.feed.AnnouncementId, String]],
            ria.async.Future, function updateAnnouncement(id, name) {
                return this.post('Announcement/Update.json', chlk.models.feed.Announcement, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.feed.AnnouncementId, String]],
            ria.async.Future, function saveAnnouncement(id_, name) {
                if (id_ && id_.valueOf()) return this.updateAnnouncement(id_);
                return this.addAnnouncement(name);
            },

            [[chlk.models.feed.AnnouncementId]],
            ria.async.Future, function removeAnnouncement(id) {
                return this.post('Announcement/Delete.json', chlk.models.feed.Announcement, {
                    announcementId: id.valueOf()
                });
            },
            [[chlk.models.feed.AnnouncementId]],
            ria.async.Future, function getAnnouncement(id) {
                return this.post('Announcement/Info.json', chlk.models.feed.Announcement, {
                    announcementId: id.valueOf()
                });
            }
        ])
});