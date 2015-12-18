REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.discipline.ClassDisciplineSummaryViewData');
REQUIRE('chlk.models.discipline.ClassDisciplinesViewData');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassDisciplinesSummary*/
    CLASS(
        'ClassDisciplinesSummary', EXTENDS(chlk.models.classes.Class), [

            chlk.models.discipline.ClassDisciplinesViewData, 'disciplines',

            chlk.models.discipline.ClassDisciplineSummaryViewData, 'stats',

            [[chlk.models.id.ClassId, String, chlk.models.discipline.ClassDisciplinesViewData, chlk.models.discipline.ClassDisciplineSummaryViewData]],
            function $(id_, name_, disciplines_, stats_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(name_)
                    this.setName(name_);
                if(disciplines_)
                    this.setDisciplines(disciplines_);
                if(stats_)
                    this.setStats(stats_);
            }
        ]);
});
