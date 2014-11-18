REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.StudentWithAvg*/
    CLASS(
        UNSAFE, 'StudentWithAvg', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.studentInfo = SJX.fromDeserializable(raw.studentinfo, chlk.models.people.ShortUserInfo);
                this.withdrawn = SJX.fromValue(raw.iswithdrawn, Boolean);
                this.avg = SJX.fromValue(raw.avg, Number);
            },

            READONLY, chlk.models.people.ShortUserInfo, 'studentInfo',
            READONLY, Boolean, 'withdrawn',
            READONLY, Number, 'avg'
        ]);
});
