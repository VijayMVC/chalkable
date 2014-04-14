REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.DisciplineId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.people.ShortUserInfo')
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.Discipline*/
    CLASS('Discipline', [

        chlk.models.id.DisciplineId, 'id',

        [ria.serialize.SerializeProperty('studentid')],
        chlk.models.id.SchoolPersonId, 'studentId',

        chlk.models.people.ShortUserInfo, 'student',

        [ria.serialize.SerializeProperty('teacherid')],
        chlk.models.id.SchoolPersonId, 'teacherId',

        [ria.serialize.SerializeProperty('classid')],
        chlk.models.id.ClassId, 'classId',

        [ria.serialize.SerializeProperty('classname')],
        String, 'className',

        [ria.serialize.SerializeProperty('disciplinetypes')],
        ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',

        chlk.models.common.ChlkDate, 'date',

        String, 'description',

        Boolean, 'editable'
    ]);
});