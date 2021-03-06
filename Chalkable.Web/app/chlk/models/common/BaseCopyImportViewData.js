REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.common.BaseCopyImportViewData*/
    CLASS(
        UNSAFE, 'BaseCopyImportViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.ids = SJX.fromValue(raw.ids, String);
                this.toClassId = SJX.fromValue(raw.toClassId, chlk.models.id.ClassId);
                this.classId = SJX.fromValue(raw.classId || raw.classid, chlk.models.id.ClassId);
                this.copyStartDate = SJX.fromDeserializable(raw.copyStartDate, chlk.models.common.ChlkDate);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.copyToYearName = SJX.fromValue(raw.copyToYearName, String);
                this.toClassName = SJX.fromValue(raw.toClassName, String);
                this.requestId = SJX.fromValue(raw.requestId, String);
            },

            chlk.models.id.ClassId, 'toClassId',

            chlk.models.id.ClassId, 'classId',

            chlk.models.common.ChlkDate, 'copyStartDate',

            String, 'ids',

            String, 'requestId',

            String, 'copyToYearName',

            String, 'toClassName',

            String, 'submitType'
        ]);
});
