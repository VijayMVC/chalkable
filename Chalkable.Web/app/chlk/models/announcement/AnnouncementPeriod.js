REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.models.announcement.LessonPlanViewData');
REQUIRE('chlk.models.announcement.SupplementalAnnouncementViewData');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AnnouncementPeriod*/
    CLASS(
        'AnnouncementPeriod', EXTENDS(chlk.models.Popup), IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.period = SJX.fromDeserializable(raw.period, chlk.models.period.Period);
                this.roomNumber = SJX.fromValue(raw.roomnumber, Number);
                if(raw.index)
                    this.index = SJX.fromValue(raw.index, Number);
                if(raw.date)
                    this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
                this.lessonPlans = SJX.fromArrayOfDeserializables(raw.lessonplans, chlk.models.announcement.LessonPlanViewData);
                this.announcements = SJX.fromArrayOfDeserializables(raw.announcements, chlk.models.announcement.ClassAnnouncementViewData);
                this.supplementalAnnouncements = SJX.fromArrayOfDeserializables(raw.supplementalannouncements, chlk.models.announcement.SupplementalAnnouncementViewData);
            },

            chlk.models.period.Period, 'period',

            Number, 'roomNumber',

            Number, 'index',

            chlk.models.common.ChlkDate, 'date',

            ArrayOf(chlk.models.announcement.LessonPlanViewData), 'lessonPlans',

            ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), 'announcements',

            ArrayOf(chlk.models.announcement.SupplementalAnnouncementViewData), 'supplementalAnnouncements',

            chlk.models.id.ClassId, 'selectedClassId'
        ]);
});
