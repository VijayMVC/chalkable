REQUIRE('chlk.models.id.SchoolId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.SchoolStatistic*/
    CLASS(
        'SchoolStatistic', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.SchoolId, 'id',

            String, 'name',

            Number, 'absences',

            Number, 'infractionsCount',

            Number, 'avg',

            Boolean, 'absencesPassed',

            Boolean, 'infractionsPassed',

            Boolean, 'avgPassed',

            Number, 'studentsCount',

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.SchoolId);
                this.name = SJX.fromValue(raw.name, String);
                this.absences = SJX.fromValue(raw.attendancescount, Number);
                this.infractionsCount = SJX.fromValue(raw.disciplinescount, Number);
                this.avg = SJX.fromValue(raw.average, Number);
                this.absencesPassed = SJX.fromValue(raw.absencespassed, Boolean);
                this.infractionsPassed = SJX.fromValue(raw.infractionspassed, Boolean);
                this.avgPassed = SJX.fromValue(raw.avgpassed, Boolean);
                this.studentsCount = SJX.fromValue(raw.studentscount, Number);
            }
        ]);
});
