REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.id.StudentAnnouncementId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradingService*/
    CLASS(
        'GradingService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.StudentAnnouncementId, Number, String, Boolean]],
            ria.async.Future, function updateItem(studentAnnouncementId, gradeValue, comment, dropped) {
                return this.get('Grading/UpdateItem', chlk.models.announcement.StudentAnnouncement, {
                    studentAnnouncementId: studentAnnouncementId.valueOf(),
                    gradeValue: gradeValue,
                    comment: comment,
                    dropped: dropped
                });
            }
        ])
});