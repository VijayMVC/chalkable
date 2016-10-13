REQUIRE('chlk.models.id.HealthFormId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.student.StudentHealthFormViewData*/
    CLASS(
        'StudentHealthFormViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.HealthFormId);
                this.name = SJX.fromValue(raw.name, String);
                this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
                this.verifiedDate = SJX.fromDeserializable(raw.verifieddate, chlk.models.common.ChlkDate);
            },

            chlk.models.id.HealthFormId, 'id',

            String, 'name',

            chlk.models.id.SchoolPersonId, 'studentId',

            chlk.models.common.ChlkDate, 'verifiedDate'
        ]);
});
