REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    var SJX = ria.serialize.SJX;
    /**@class chlk.models.student.StudentContact*/

    CLASS('StudentContact', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.people.User, 'personInfo',
        Boolean, 'familyMember',
        String, 'relationshipName',

        VOID, function deserialize(raw) {
            this.personInfo = SJX.fromDeserializable(raw.personinfo, chlk.models.people.User);
            this.familyMember = SJX.fromValue(raw.familymember, Boolean);
            this.relationshipName = SJX.fromValue(raw.relationshipname, String);
        }
    ]);
});