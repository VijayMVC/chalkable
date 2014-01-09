REQUIRE('chlk.models.discipline.DisciplineStudents');
REQUIRE('chlk.models.common.ChartItem');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    /** @class chlk.models.discipline.AdminDisciplineSummary*/
    CLASS(
       'AdminDisciplineSummary', EXTENDS(chlk.models.common.PageWithGrades), [
            [ria.serialize.SerializeProperty('daystats')],
            ArrayOf(chlk.models.common.ChartItem), 'dayStats',

            [ria.serialize.SerializeProperty('mpstats')],
            ArrayOf(chlk.models.common.ChartItem), 'mpStats',

            [ria.serialize.SerializeProperty('disciplinesbytype')],
            ArrayOf(chlk.models.discipline.DisciplineStudents), 'disciplinesByType',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('markingperiodname')],
            String, 'markingPeriodName',

            [ria.serialize.SerializeProperty('nowcount')],
            Number, 'nowCount',

            [ria.serialize.SerializeProperty('daycount')],
            Number, 'dayCount',

            [ria.serialize.SerializeProperty('mpcount')],
            Number, 'mpCount',

            String, 'gradeLevelsIds'
        ]);
});