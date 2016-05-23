REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.announcement.post', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.post.AnnouncementImportPostViewData*/
    CLASS(
        UNSAFE, 'AnnouncementImportPostViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.announcementsToCopy = SJX.fromValue(raw.announcementsToCopy, String);
                this.toClassId = SJX.fromValue(raw.toClassId, chlk.models.id.ClassId);
                this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
                this.copyStartDate = SJX.fromDeserializable(raw.copyStartDate, chlk.models.common.ChlkDate);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.requestId = SJX.fromValue(raw.requestId, String);
            },

            chlk.models.id.ClassId, 'toClassId',

            chlk.models.id.ClassId, 'classId',

            chlk.models.common.ChlkDate, 'copyStartDate',

            String, 'announcementsToCopy',

            String, 'requestId',

            String, 'submitType'
        ]);
});
