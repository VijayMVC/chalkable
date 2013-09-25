REQUIRE('chlk.models.discipline.DisciplineType');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /**@class chlk.models.discipline.DisciplineTypeSummary*/
    CLASS('DisciplineTypeSummary', EXTENDS(chlk.models.discipline.DisciplineType),[
        Number, 'count',

        [ria.serialize.SerializeProperty('periodorder')],
        Number, 'periodOrder',

        String, function getPeriodName(){
            var order = this.getPeriodOrder();
            if(!order) return null;
            return getSerial(order);
        }
    ]);
});