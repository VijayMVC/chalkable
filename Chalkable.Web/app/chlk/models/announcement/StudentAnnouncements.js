REQUIRE('chlk.models.class.ClassForTopBar');
REQUIRE('chlk.models.announcement.StudentAnnouncement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StudentAnnouncements*/
    CLASS(
        'StudentAnnouncements', [
            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId',

            [ria.serialize.SerializeProperty('announcmenttitel')],
            String, 'announcementTitle',

            [ria.serialize.SerializeProperty('classavg')],
            Number, 'classAvg',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className',

            [ria.serialize.SerializeProperty('courseid')],
            chlk.models.class.CourseId, 'courseId',

            [ria.serialize.SerializeProperty('gradedstudentcount')],
            Number, 'gradedStudentCount',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',

            ArrayOf(chlk.models.announcement.StudentAnnouncement), 'items'
        ]);
});
