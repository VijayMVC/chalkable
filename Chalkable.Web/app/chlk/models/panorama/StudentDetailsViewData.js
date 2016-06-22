REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.people.EthnicityViewData');

NAMESPACE('chlk.models.panorama', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.panorama.StudentDetailsViewData*/
    CLASS(
        UNSAFE, 'StudentDetailsViewData', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),  [

        OVERRIDE, VOID, function deserialize(raw){
            BASE(raw);
            this.hispanic = SJX.fromValue(raw.hispanic, Boolean);
            this.retainedFromPrevSchoolYear = SJX.fromValue(raw.isretainedfromprevschoolyear, Boolean);
            this.IEPActive = SJX.fromValue(raw.isiepactive, Boolean);
            this.ethnicity = SJX.fromDeserializable(raw.ethnicity, chlk.models.people.EthnicityViewData);
            this.gradeAvg = SJX.fromValue(raw.gradeavg, Number);
            this.absences = SJX.fromValue(raw.absences, Number);
            this.discipline = SJX.fromValue(raw.discipline, Number);
        },

            Boolean, 'hispanic',
            Boolean, 'retainedFromPrevSchoolYear',
            Boolean, 'IEPActive',
            chlk.models.people.EthnicityViewData, 'ethnicity',
            Number, 'gradeAvg',
            Number, 'absences',
            Number, 'discipline'
    ]);
});
