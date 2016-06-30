REQUIRE('chlk.models.student.StudentSummary');
REQUIRE('chlk.models.people.HealthCondition');
REQUIRE('chlk.models.student.StudentContact');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.student.StudentSummary),[

        ArrayOf(chlk.models.student.StudentContact), 'studentContacts',
        chlk.models.grading.GradeLevel, 'gradeLevel',
        Number, 'age',
        Boolean, 'hispanic',
        Boolean, 'IEPActive',
        Boolean, 'title1Eligible',
        Boolean, 'section504',
        Boolean, 'homeless',
        Boolean, 'immigrant',
        Boolean, 'lep',
        Boolean, 'foreignExchange',
        Boolean, 'retained',
        Number, 'stateIDNumber',
        Number, 'alternateStudentNumber',
        Number, 'studentNumber',
        chlk.models.common.NameId, 'language',
        chlk.models.common.NameId, 'nationality',
        chlk.models.common.ChlkDate, 'originalEnrollmentDate',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.studentContacts = SJX.fromArrayOfDeserializables(raw.studentcontacts, chlk.models.student.StudentContact);
            this.gradeLevel = SJX.fromDeserializable(raw.gradelevel, chlk.models.grading.GradeLevel);
            this.age = SJX.fromValue(raw.age, Number);
            this.hispanic = SJX.fromValue(raw.hispanic, Boolean);
            this.IEPActive = SJX.fromValue(raw.isiepactive, Boolean);
            this.title1Eligible = SJX.fromValue(raw.istitle1eligible, Boolean);
            this.section504 = SJX.fromValue(raw.section504, Boolean);
            this.homeless = SJX.fromValue(raw.ishomeless, Boolean);
            this.immigrant = SJX.fromValue(raw.isimmigrant, Boolean);
            this.lep = SJX.fromValue(raw.lep, Boolean);
            this.foreignExchange = SJX.fromValue(raw.isforeignexchange, Boolean);
            this.stateIDNumber = SJX.fromValue(raw.isforeignexchange, Boolean);
            this.retained = SJX.fromValue(raw.isretained, Number);
            this.alternateStudentNumber = SJX.fromValue(raw.alternatestudentnumber, Number);
            this.studentNumber = SJX.fromValue(raw.studentnumber, Number);
            this.language = SJX.fromDeserializable(raw.language, chlk.models.common.NameId);
            this.nationality = SJX.fromDeserializable(raw.nationality, chlk.models.common.NameId);
            this.originalEnrollmentDate = SJX.fromDeserializable(raw.originalenrollmentdate, chlk.models.common.ChlkDate);
        }
    ]);
});