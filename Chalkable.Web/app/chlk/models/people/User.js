REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.Address');
REQUIRE('chlk.models.people.Phone');
REQUIRE('chlk.models.people.Role');
REQUIRE('chlk.models.people.Claim');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.User*/
    CLASS(
        UNSAFE, 'User', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),  [

        OVERRIDE, VOID, function deserialize(raw){
            BASE(raw);
            this.active = SJX.fromValue(raw.active, Boolean);
            this.address = SJX.fromDeserializable(raw.address, chlk.models.people.Address);
            this.grade = SJX.fromValue(raw.grade, String);
            this.localId = SJX.fromValue(raw.localid, String);
            this.schoolId = SJX.fromValue(raw.schoolid, Number);
            this.birthDate = SJX.fromDeserializable(raw.birthdate, chlk.models.common.ChlkDate);
            this.birthDateText = SJX.fromValue(raw.birthdatetext, String);
            this.phones = SJX.fromArrayOfDeserializables(raw.phones, chlk.models.people.Phone);
            this.ableEdit = SJX.fromValue(raw.ableedit, Boolean);
            this.primaryPhone = SJX.fromDeserializable(raw.primaryphone, chlk.models.people.Phone);
            this.homePhone = SJX.fromDeserializable(raw.homePhone, chlk.models.people.Phone);
            this.addressesValue = SJX.fromValue(raw.addressesvalue, String);
            this.phonesValue = SJX.fromValue(raw.phonesvalue, String);
            this.index = SJX.fromValue(raw.index, Number);
            this.selected = SJX.fromValue(raw.selected, Boolean);
            this.demoUser = SJX.fromValue(raw.demouser, Boolean);
            this.claims = SJX.fromArrayOfDeserializables(raw.claims, chlk.models.people.Claim);
        },
        Boolean, 'active',
        chlk.models.people.Address, 'address',
        String, 'grade',
        String, 'localId',
        Number, 'schoolId',
        chlk.models.common.ChlkDate, 'birthDate',
        String, 'birthDateText',
        ArrayOf(chlk.models.people.Phone), 'phones',
        Boolean, 'ableEdit',
        chlk.models.people.Phone, 'primaryPhone',
        chlk.models.people.Phone, 'homePhone',
        String, 'addressesValue',
        String, 'phonesValue',
        Number, 'index',
        Boolean, 'selected',
        Boolean, 'demoUser',

        ArrayOf(chlk.models.people.Claim), 'claims',

        [[chlk.models.people.UserPermissionEnum]],
        Boolean, function hasPermission_(userPermission){
            var claims = this.getClaims();
            return claims && claims.length > 0
                && claims.filter(function(claim){return claim.hasPermission(userPermission); }).length > 0;
        },

        [[String, String, chlk.models.id.SchoolPersonId]],
        function $(firstName_, lastName_, id_){
            BASE(firstName_, lastName_, id_);
        }
    ]);
});
