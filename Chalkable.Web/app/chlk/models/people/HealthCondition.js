REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.HealthConditionId');

NAMESPACE('chlk.models.people', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.HealthCondition*/
    CLASS(
        UNSAFE, 'HealthCondition', IMPLEMENTS(ria.serialize.IDeserializable), [

            [[Object]],
            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.HealthConditionId);
                this.name = SJX.fromValue(raw.name, String);
                this.description = SJX.fromValue(raw.description, String);
                this.treatment = SJX.fromValue(raw.treatment, String);
                this.alert = SJX.fromValue(raw.isalert, Boolean);
            },

            READONLY, chlk.models.id.HealthConditionId, 'id',
            READONLY, String, 'name',
            READONLY, String, 'description',
            READONLY, String, 'treatment',
            READONLY, Boolean, 'alert'
        ]);
});