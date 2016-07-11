REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.schoolYear.ScheduleSection');
REQUIRE('chlk.models.announcement.LessonPlanViewData');
REQUIRE('chlk.models.announcement.AdminAnnouncementViewData');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.models.Popup');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.MonthItem*/
    CLASS(
        'MonthItem', EXTENDS(chlk.models.Popup), [
            Number, 'day',
            [ria.serialize.SerializeProperty('iscurrentmonth')],
            Boolean, 'currentMonth',

            [ria.serialize.SerializeProperty('issunday')],
            Boolean, 'sunday',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('schedulesection')],
            chlk.models.schoolYear.ScheduleSection, 'scheduleSection',

            [ria.serialize.SerializeProperty('lessonplans')],
            ArrayOf(chlk.models.announcement.LessonPlanViewData), 'lessonPlans',

            [ria.serialize.SerializeProperty('adminannouncements')],
            ArrayOf(chlk.models.announcement.AdminAnnouncementViewData), 'adminAnnouncements',

            ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), 'announcements',

            String, 'todayClassName',

            String, 'role',

            Number, 'annLimit',

            String, 'className',

            Array, 'itemsArray',

            Boolean, 'noPlusButton',

            chlk.models.id.ClassId, 'selectedClassId'
        ]);
});
