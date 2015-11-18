REQUIRE('chlk.models.id.SchoolId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.admin', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.admin.BaseStatistic*/
    CLASS(
        GENERIC('TId'),
        'BaseStatistic', IMPLEMENTS(ria.serialize.IDeserializable), [
            TId, 'id',

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
                this.absences = SJX.fromValue(raw.attendancescount || 100 * Math.random(100).toFixed(2), Number);
                this.infractionsCount = SJX.fromValue(raw.disciplinescount || 100 * Math.random(100).toFixed(2), Number);
                this.avg = SJX.fromValue(raw.average || 100 * Math.random(100).toFixed(2), Number);
                this.absencesPassed = SJX.fromValue(raw.absencespassed, Boolean);
                this.infractionsPassed = SJX.fromValue(raw.infractionspassed, Boolean);
                this.avgPassed = SJX.fromValue(raw.avgpassed , Boolean);
                this.studentsCount = SJX.fromValue(raw.studentscount, Number);
            }
        ]);
});
