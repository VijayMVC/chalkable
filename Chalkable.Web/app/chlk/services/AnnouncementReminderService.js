REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ReminderId');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.announcement.Reminder');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.AnnouncementReminderService*/
    CLASS(
        'AnnouncementReminderService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.AnnouncementId, Number]],
            ria.async.Future, function addReminder(announcementId, before) {
                return this.get('AnnouncementReminder/AddReminder.json', chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf(),
                    before: before
                });
            },

            [[chlk.models.id.ReminderId, Number]],
            ria.async.Future, function editReminder(announcementReminderId, before) {
                return this.get('AnnouncementReminder/EditReminder.json', chlk.models.announcement.Reminder, {
                    announcementReminderId: announcementReminderId.valueOf(),
                    before: before
                });
            },

            [[chlk.models.id.ReminderId]],
            ria.async.Future, function deleteReminder(announcementReminderId) {
                return this.get('AnnouncementReminder/DeleteReminder.json', chlk.models.announcement.Announcement, {
                    announcementReminderId: announcementReminderId.valueOf()
                });
            }
        ])
});