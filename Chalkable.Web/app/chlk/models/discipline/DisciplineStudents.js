REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.discipline.DisciplineType');

NAMESPACE('chlk.models.discipline', function(){

    /** @class chlk.models.discipline.DisciplineStudents*/
    CLASS('DisciplineStudents', [
        ArrayOf(chlk.models.people.User), 'students',

        chlk.models.discipline.DisciplineType, 'type'
    ]);
});