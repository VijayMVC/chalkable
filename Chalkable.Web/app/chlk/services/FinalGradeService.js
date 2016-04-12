REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.FinalGradeService */
    CLASS(
        'FinalGradeService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function dropAnnouncement(announcementId) {
                return this.get('FinalGrade/DropAnnouncement.json', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function unDropAnnouncement(announcementId) {
                return this.get('FinalGrade/UndropAnnouncement.json', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            }
        ])
});