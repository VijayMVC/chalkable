REQUIRE('chlk.models.id.SchoolId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.admin', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.admin.TeacherSortTypeEnum*/
    ENUM('TeacherSortTypeEnum', {
        TEACHER_ASC: 0,
        TEACHER_DESC: 1,
        STUDENTS_ASC: 2,
        STUDENTS_DESC: 3,
        ATTENDANCE_ASC: 4,
        ATTENDANCE_DESC: 5,
        DISCIPLINE_ASC: 6,
        DISCIPLINE_DESC: 7,
        GRADES_ASC: 8,
        GRADES_DESC: 9
    });

    /** @class chlk.models.admin.ClassSortTypeEnum*/
    ENUM('ClassSortTypeEnum', {
        CLASS_ASC: 0,
        CLASS_DESC: 1,
        TEACHER_ASC: 2,
        TEACHER_DESC: 3,
        STUDENTS_ASC: 4,
        STUDENTS_DESC: 5,
        ATTENDANCE_ASC: 6,
        ATTENDANCE_DESC: 7,
        DISCIPLINE_ASC: 8,
        DISCIPLINE_DESC: 9,
        GRADES_ASC: 10,
        GRADES_DESC: 11,
        PERIOD_ASC: 12,
        PERIOD_DESC: 13
    });

    /** @class chlk.models.admin.SchoolSortTypeEnum*/
    ENUM('SchoolSortTypeEnum', {
        SCHOOL_ASC: 0,
        SCHOOL_DESC: 1,
        STUDENTS_ASC: 2,
        STUDENTS_DESC: 3,
        ATTENDANCE_ASC: 4,
        ATTENDANCE_DESC: 5,
        DISCIPLINE_ASC: 6,
        DISCIPLINE_DESC: 7,
        GRADES_ASC: 8,
        GRADES_DESC: 9
    });

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

            Number, 'presence',

            function getAbsencesClass(){
                if(this.getAbsences() !== null && (this.getAbsences() / this.getStudentsCount() > 0.1))
                    return 'red';

                return 'green';
            },

            function getInfractionsClass(){
                if(this.getInfractionsCount() !== null && (this.getInfractionsCount() / this.getStudentsCount() > 0.01))
                    return 'red';

                return 'green';
            },

            function getAvgClass(){
                if(this.getAvg() < 65)
                    return 'red';

                return 'green';
            },

            function toFixed_(raw_) {
                    return raw_ && parseFloat(raw_).toFixed(2);
            },

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, this.getSpecsOf('TId'));
                this.name = SJX.fromValue(raw.name, String);
                this.absences = SJX.fromValue(raw.absencecount, Number);
                this.presence = SJX.fromValue(this.toFixed_(raw.presence), Number);
                this.infractionsCount = SJX.fromValue(raw.disciplinescount, Number);
                this.avg = SJX.fromValue(this.toFixed_(raw.average), Number);
                this.absencesPassed = SJX.fromValue(raw.absencespassed, Boolean);
                this.infractionsPassed = SJX.fromValue(raw.infractionspassed, Boolean);
                this.avgPassed = SJX.fromValue(raw.avgpassed , Boolean);
                this.studentsCount = SJX.fromValue(raw.studentscount, Number);
            }
        ]);
});
