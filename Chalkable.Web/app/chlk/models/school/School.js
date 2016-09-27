REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.school.Timezone');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.SchoolYearId');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.School*/
    CLASS(
        'School', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.SchoolId, 'id',

            String, 'name',

            Number, 'localId',

            Number, 'ncesId',

            String, 'schoolType',

            String, 'schoolUrl',

            Boolean,'sendEmailNotifications',

            String, 'timezoneId',

            ArrayOf(chlk.models.school.Timezone), 'timezones',

            chlk.models.id.DistrictId, 'districtId',

            chlk.models.id.SchoolYearId, 'schoolYearId',

            chlk.models.common.ChlkDate, 'studyCenterEnabledTill',

            Boolean, 'assessmentEnabled',

            READONLY, Boolean, 'upgraded',

            Boolean, function isUpgraded(){
                var upgradedDate = this.getStudyCenterEnabledTill();
                return upgradedDate && upgradedDate.toStandardFormat() >=  (new chlk.models.common.ChlkDate()).toStandardFormat();
            },

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.SchoolId);
                this.name = SJX.fromValue(raw.name, String);
                this.localId = SJX.fromValue(raw.localid, Number);
                this.ncesId = SJX.fromValue(raw.ncesid, Number);
                this.schoolType = SJX.fromValue(raw.schooltype, String);
                this.schoolUrl = SJX.fromValue(raw.schoolurl, String);
                this.sendEmailNotifications = SJX.fromValue(raw.sendemailnotofications, Boolean);
                this.timezoneId = SJX.fromValue(raw.timezoneid, String);
                this.timezones = SJX.fromArrayOfDeserializables(raw.timezones, chlk.models.school.Timezone);
                this.districtId = SJX.fromValue(raw.districtid, chlk.models.id.DistrictId);
                this.schoolYearId = SJX.fromValue(raw.schoolyearid, chlk.models.id.SchoolYearId);
                this.studyCenterEnabledTill = SJX.fromDeserializable(raw.studycenterenabledtill, chlk.models.common.ChlkDate);
                this.assessmentEnabled = SJX.fromValue(raw.isassessmentenabled, Boolean);
            }
        ]);
});
