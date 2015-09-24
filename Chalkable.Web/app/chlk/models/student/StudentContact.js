REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    var SJX = ria.serialize.SJX;
    /**@class chlk.models.student.StudentContact*/

    CLASS('StudentContact', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.people.User, 'personInfo',
        Boolean, 'familyMember',
        String, 'relationshipName',
        Boolean, 'emergencyContact',
        Boolean, 'responsibleForBill',
        Boolean, 'allowedToPickup',
        Boolean, 'custodian',
        Boolean, 'receivesMailings',
        Boolean, 'receivesBill',

        VOID, function deserialize(raw) {
            this.personInfo = SJX.fromDeserializable(raw.personinfo, chlk.models.people.User);
            this.familyMember = SJX.fromValue(raw.isfamilymember, Boolean);
            this.relationshipName = SJX.fromValue(raw.relationshipname, String);
            this.emergencyContact = SJX.fromValue(raw.isemergencycontact, Boolean);
            this.responsibleForBill = SJX.fromValue(raw.isresponsibleforbill, Boolean);
            this.allowedToPickup = SJX.fromValue(raw.isallowedtopickup, Boolean);
            this.custodian = SJX.fromValue(raw.iscustodian, Boolean);
            this.receivesMailings = SJX.fromValue(raw.receivesmailings, Boolean);
            this.receivesBill = SJX.fromValue(raw.receivesbill, Boolean);
        }
    ]);
});