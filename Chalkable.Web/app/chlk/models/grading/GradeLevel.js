REQUIRE('chlk.models.id.GradeLevelId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradeLevel*/
    CLASS(
        'GradeLevel', [
            chlk.models.id.GradeLevelId, 'id',
            String, 'name',
            Number, 'number',
            String, 'serialPart',
            String, 'fullText',

            String, function getFullText(){
                //var number = this.getNumber();
                //return number ? getSerial(number) : this.getName();
                return getSerial(this.getName());
            }
        ]);
});
