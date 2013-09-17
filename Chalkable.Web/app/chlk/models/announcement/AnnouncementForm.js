REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.classes.ClassForWeekMask');
REQUIRE('chlk.models.announcement.AnnouncementCreate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementForm*/
    CLASS(
        'AnnouncementForm', EXTENDS(chlk.models.announcement.AnnouncementCreate), [
            chlk.models.classes.ClassesForTopBar, 'topData', //todo: rename
            chlk.models.classes.ClassForWeekMask, 'classInfo',
            Number, 'selectedTypeId',
            Array, 'reminders'
        ]);
});
