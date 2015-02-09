REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.HealthCondition');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        ArrayOf(chlk.models.people.User), 'parents',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.parents = SJX.fromArrayOfDeserializables(raw.parents, chlk.models.people.User);
        }
    ]);
});