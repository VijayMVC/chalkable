REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.id.GalleryCategoryId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.LessonPlanViewData*/
    CLASS(
        UNSAFE, 'LessonPlanViewData',
                EXTENDS(chlk.models.announcement.BaseAnnouncementViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.classId = SJX.fromValue(Number(raw.classid), chlk.models.id.ClassId);
                this.shortClassName = SJX.fromValue(raw.classname, String);
                this.className = SJX.fromValue(raw.fullclassname, String);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.galleryCategoryId = SJX.fromValue(raw.gallerycategoryid, chlk.models.id.GalleryCategoryId);
            },

            chlk.models.common.ChlkDate, 'startDate',
            chlk.models.common.ChlkDate, 'endDate',
            chlk.models.id.ClassId, 'classId',
            String, 'shortClassName',
            String, 'className',
            Boolean, 'hiddenFromStudents',
            chlk.models.id.GalleryCategoryId, 'galleryCategoryId'
        ]);
});
