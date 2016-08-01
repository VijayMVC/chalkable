REQUIRE('chlk.models.classes.Class');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.classes.ClassGradingSummary*/
    CLASS(
        GENERIC('TItem'),
        UNSAFE, 'ClassGradingSummary', EXTENDS(chlk.models.classes.Class), IMPLEMENTS(ria.serialize.IDeserializable),[
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.alphaGrades = SJX.fromArrayOfDeserializables(raw.alphagrades, chlk.models.grading.AlphaGrade);
                this.alphaGradesForStandards = SJX.fromArrayOfDeserializables(raw.alphagradesforstandards, chlk.models.grading.AlphaGrade);
                this.schoolYear = SJX.fromDeserializable(raw.schoolyear, chlk.models.schoolYear.Year);
            },

            TItem, 'gradingPart',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGradesForStandards',

            chlk.models.schoolYear.Year, 'schoolYear'
    ]);
});