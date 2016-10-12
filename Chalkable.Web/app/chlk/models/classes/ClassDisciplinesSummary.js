REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.discipline.ClassDisciplineStatsViewData');
REQUIRE('chlk.models.discipline.ClassDisciplinesViewData');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassDisciplinesSummary*/
    CLASS(
        'ClassDisciplinesSummary', EXTENDS(chlk.models.classes.Class), [

            chlk.models.discipline.ClassDisciplinesViewData, 'disciplines',

            chlk.models.discipline.ClassDisciplineStatsViewData, 'stats'
        ]);
});
