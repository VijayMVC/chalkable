REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.LpGalleryCategoryId');

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
                this.galleryCategoryId = SJX.fromValue(raw.lpgallerycategoryid, chlk.models.id.LpGalleryCategoryId);
                this.categoryName = SJX.fromValue(raw.categoryname, String);
                this.inGallery = SJX.fromValue(raw.ingallery, Boolean);
                this.galleryOwnerRef = SJX.fromValue(raw.galleryownerref, chlk.models.id.SchoolPersonId);
            },

            chlk.models.common.ChlkDate, 'startDate',
            chlk.models.common.ChlkDate, 'endDate',
            chlk.models.id.ClassId, 'classId',
            String, 'shortClassName',
            String, 'className',
            Boolean, 'hiddenFromStudents',
            Boolean, 'inGallery',
            chlk.models.id.LpGalleryCategoryId, 'galleryCategoryId',
            String, 'categoryName',
            chlk.models.id.SchoolPersonId, 'galleryOwnerRef',

            function getPercents(){
                if(!this.endDate || !this.startDate)
                    return 0;
                var len = getDateDiffInDays(this.startDate.getDate(), this.endDate.getDate()) + 1;
                var greenDaysLen = getDateDiffInDays(this.startDate.getDate(), getDate()) + 1;
                if(greenDaysLen < 1)
                    return 0;
                if(greenDaysLen >= len)
                    return 100;
                return Math.round(greenDaysLen * 100 / len);
            }
        ]);
});
