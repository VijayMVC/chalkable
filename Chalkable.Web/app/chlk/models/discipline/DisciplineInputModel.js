REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');


NAMESPACE('chlk.models.discipline', function(){

    /** @class chlk.models.discipline.DisciplineInputModel*/

    CLASS(
        'DisciplineInputModel', [

            chlk.models.id.SchoolPersonId, 'personId',
            chlk.models.common.ChlkDate, 'disciplineDate'
    ]);
});