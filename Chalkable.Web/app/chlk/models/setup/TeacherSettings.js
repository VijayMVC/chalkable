REQUIRE('chlk.models.class.ClassesForTopBar');
REQUIRE('chlk.models.class.ClassForWeekMask');
REQUIRE('chlk.models.announcement.AnnouncementCreate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.setup.TeacherSettings*/
    CLASS(
        'TeacherSettings', EXTENDS(chlk.models.announcement.AnnouncementCreate), [
            chlk.models.class.ClassesForTopBar, 'topData',
            chlk.models.class.ClassForWeekMask, 'classInfo',
            Number, 'selectedTypeId'
        ]);
});
