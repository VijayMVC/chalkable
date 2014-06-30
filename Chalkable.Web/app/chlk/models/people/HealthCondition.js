REQUIRE('chlk.models.id.HealthConditionId');

NAMESPACE('chlk.models.people', function(){
    "use strict";

    /**@class chlk.models.student.HealthCondition*/

    CLASS('HealthCondition', [

        chlk.models.id.HealthConditionId, 'id',
        String, 'name',
        String, 'description',
        String, 'treatment',

        [ria.serialize.SerializeProperty('isalert')],
        Boolean, 'alert'
    ]);
});