REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.DisciplineId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.DisciplineId');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.discipline.Discipline*/
    CLASS(
        UNSAFE, 'Discipline', IMPLEMENTS(ria.serialize.IDeserializable), [


        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.DisciplineId);
            this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
            if(raw.student)
                this.student = SJX.fromDeserializable(raw.student, chlk.models.people.ShortUserInfo);
            this.teacherId = SJX.fromValue(raw.teacherid, chlk.models.id.SchoolPersonId);
            this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            this.className = SJX.fromValue(raw.classname, String);
            this.disciplineTypes = SJX.fromArrayOfDeserializables(raw.disciplinetypes, chlk.models.discipline.DisciplineType);
            this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
            this.description = SJX.fromValue(raw.description, String);
            this.editable = SJX.fromValue(raw.editable, Boolean);
        },

        chlk.models.id.DisciplineId, 'id',
        chlk.models.id.SchoolPersonId, 'studentId',
        chlk.models.people.ShortUserInfo, 'student',
        chlk.models.id.SchoolPersonId, 'teacherId',
        chlk.models.id.ClassId, 'classId',
        String, 'className',
        ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',
        chlk.models.common.ChlkDate, 'date',
        String, 'description',
        Boolean, 'editable',

    ]);
});