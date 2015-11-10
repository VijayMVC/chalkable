REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.AnnouncementPeriod');
REQUIRE('chlk.models.announcement.LessonPlanViewData');
REQUIRE('chlk.models.announcement.AdminAnnouncementViewData');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.models.Popup');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;


    /** @class chlk.models.calendar.announcement.WeekItem*/
    CLASS(
        'WeekItem', EXTENDS(chlk.models.Popup), IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
                this.day = SJX.fromValue(raw.day, Number);
                this.dayOfWeek = SJX.fromValue(raw.dayofweek, Number);
                this.sunday = SJX.fromValue(raw.sunday, Boolean);
                this.todayClassName = SJX.fromValue(raw.todayclassname, String);
                this.announcementPeriods = SJX.fromArrayOfDeserializables(raw.announcementperiods, chlk.models.announcement.AnnouncementPeriod);
                this.adminAnnouncements = SJX.fromArrayOfDeserializables(raw.adminannouncements, chlk.models.announcement.AdminAnnouncementViewData);
            },

            chlk.models.common.ChlkDate, 'date',

            Number, 'day',

            Number, 'dayOfWeek',

            Boolean, 'sunday',

            String, 'todayClassName',

            chlk.models.id.ClassId, 'selectedClassId',

            ArrayOf(chlk.models.announcement.AnnouncementPeriod), 'announcementPeriods',

            ArrayOf(chlk.models.announcement.AdminAnnouncementViewData), 'adminAnnouncements',

            Boolean, 'noPlusButton'
        ]);
});
