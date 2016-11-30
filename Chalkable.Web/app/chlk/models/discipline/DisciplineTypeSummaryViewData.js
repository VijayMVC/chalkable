REQUIRE('chlk.models.discipline.DisciplineType');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.DisciplineTypeSummaryViewData*/
    CLASS('DisciplineTypeSummaryViewData', [

        Number, 'count',

        chlk.models.discipline.DisciplineType, 'type'
    ]);
});