REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.LpGalleryCategoryId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.SupplementalAnnouncementViewData*/
    CLASS(
        UNSAFE, 'SupplementalAnnouncementViewData',
                EXTENDS(chlk.models.announcement.BaseAnnouncementViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
                this.classId = SJX.fromValue(Number(raw.classid), chlk.models.id.ClassId);
                this.shortClassName = SJX.fromValue(raw.classname, String);
                this.className = SJX.fromValue(raw.fullclassname, String);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.galleryCategoryId = SJX.fromValue(raw.gallerycategoryid, chlk.models.id.LpGalleryCategoryId);
            },

            chlk.models.common.ChlkDate, 'expiresDate',
            chlk.models.id.ClassId, 'classId',
            String, 'shortClassName',
            String, 'className',
            Boolean, 'hiddenFromStudents',
            chlk.models.id.LpGalleryCategoryId, 'galleryCategoryId'
        ]);
});
