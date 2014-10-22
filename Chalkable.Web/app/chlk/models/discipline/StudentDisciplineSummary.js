REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.discipline.StudentDisciplineHoverBox');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
//REQUIRE('chlk.models.discipline.StudentDisciplineHoverBoxItem');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    /**@class chlk.models.discipline.StudentDisciplineSummary*/
    CLASS('StudentDisciplineSummary', EXTENDS(chlk.models.people.ShortUserInfo),[

        String, 'summary',

        [ria.serialize.SerializeProperty('markingperiod')],
        chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',

        [ria.serialize.SerializeProperty('disciplineboxes')],
        ArrayOf(chlk.models.discipline.StudentDisciplineHoverBox), 'disciplineBoxes'
    ]);
});