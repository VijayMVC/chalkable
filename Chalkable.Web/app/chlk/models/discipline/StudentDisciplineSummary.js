REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.discipline.StudentDisciplineHoverBoxItem');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.discipline.StudentDisciplineSummary*/
    CLASS(
        UNSAFE, 'StudentDisciplineSummary', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),[

            chlk.models.people.ShortUserInfo, 'student',

            String, 'summary',

            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            chlk.models.schoolYear.GradingPeriod, 'currentGradingPeriod',

            ArrayOf(chlk.models.common.HoverBox.OF(chlk.models.discipline.StudentDisciplineHoverBoxItem)), 'disciplineBoxes',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw.student);
                this.gradingPeriods = SJX.fromArrayOfDeserializables(raw.gradingperiods, chlk.models.schoolYear.GradingPeriod);
                this.currentGradingPeriod = SJX.fromDeserializable(raw.currentgradingperiod, chlk.models.schoolYear.GradingPeriod);
                this.student = SJX.fromDeserializable(raw.student, chlk.models.people.ShortUserInfo);
                this.summary = SJX.fromValue(raw.summary, String);
                if(raw.disciplineboxes){
                    var res = [];
                    raw.disciplineboxes.forEach(function(item){
                        res.push(SJX.fromDeserializable(item, chlk.models.common.HoverBox.OF(chlk.models.discipline.StudentDisciplineHoverBoxItem)));
                    });
                    this.disciplineBoxes = res;
                }
                //this.disciplineBoxes = SJX.fromArrayOfDeserializables(raw.disciplineboxes, chlk.models.common.HoverBox.OF(chlk.models.discipline.StudentDisciplineHoverBoxItem));
            }
    ]);
});