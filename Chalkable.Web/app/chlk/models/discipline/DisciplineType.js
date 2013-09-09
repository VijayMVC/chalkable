REQUIRE('chlk.models.id.DisciplineTypeId');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.DisciplineType*/
    CLASS('DisciplineType', [
         chlk.models.id.DisciplineTypeId, 'id',
         String, 'name',
         Number, 'score'
    ]);
});