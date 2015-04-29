REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.HealthCondition');
REQUIRE('chlk.models.student.StudentContact');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        ArrayOf(chlk.models.student.StudentContact), 'studentContacts',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.studentContacts = SJX.fromArrayOfDeserializables(raw.studentcontacts, chlk.models.student.StudentContact);
        }
    ]);
});