REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.ClassPersonId');

NAMESPACE('chlk.models.discipline', function(){

    /** @class chlk.models.discipline.Discipline*/
    CLASS('Discipline', [

        [ria.serialize.SerializeProperty('studentid')],
        chlk.models.id.SchoolPersonId, 'studentId',

        [ria.serialize.SerializeProperty('classpersonid')],
        chlk.models.id.ClassPersonId, 'classPersonId',

        [ria.serialize.SerializeProperty('classperiodid')],
        chlk.models.id.ClassPeriodId, 'classPeriodId',

        [ria.serialize.SerializeProperty('teacherid')],
        chlk.models.id.SchoolPersonId, 'teacherId',

        [ria.serialize.SerializeProperty('classname')],
        String, 'className',

        [ria.serialize.SerializeProperty('disciplinetype')],
        ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineType',

        chlk.models.common.ChlkDate, 'date',

        chlk.models.period.Period, 'period',

        String, 'description',

        String, 'summary',

        Boolean, 'editable'
    ]);
});