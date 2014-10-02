REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.grading.Mapping');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.announcement.BaseStudentAnnouncementsViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StudentAnnouncements*/
    CLASS(
        'StudentAnnouncements', EXTENDS(chlk.models.announcement.BaseStudentAnnouncementsViewData), [
            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId',

            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.templates.ModelPropertyBind],
            Number, 'chalkableAnnouncementTypeId',

            [ria.serialize.SerializeProperty('announcmenttitel')],
            String, 'announcementTitle',

            [ria.serialize.SerializeProperty('classavg')],
            Number, 'classAvg',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className',

            [ria.serialize.SerializeProperty('courseid')],
            chlk.models.id.CourseId, 'courseId',

            [ria.serialize.SerializeProperty('gradedstudentcount')],
            Number, 'gradedStudentCount',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',

            ArrayOf(chlk.models.announcement.StudentAnnouncement), 'items',

            [ria.serialize.SerializeProperty('gradingstylemapper')],
            chlk.models.grading.Mapping, 'mapping',

            [ria.serialize.SerializeProperty('showtostudents')],
            Boolean, 'showToStudents',

            chlk.models.announcement.StudentAnnouncement, 'currentItem',

            Number, 'selectedIndex'
        ]);
});
