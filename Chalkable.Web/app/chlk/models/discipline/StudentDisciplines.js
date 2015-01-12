REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.discipline', function(){

    /** @class chlk.models.discipline.StudentDisciplines*/
    CLASS('StudentDisciplines', [
        chlk.models.people.User, 'student',

        Number, 'total',

        [ria.serialize.SerializeProperty('disciplinerecordsnumber')],
        Number, 'disciplineRecordsNumber',

        String, 'summary'
    ]);
});