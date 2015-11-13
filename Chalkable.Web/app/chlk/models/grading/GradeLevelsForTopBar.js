REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradeLevelsForTopBar*/


    //TODO: refactor
    CLASS(
        'GradeLevelsForTopBar', [
            ArrayOf(chlk.models.grading.GradeLevel), 'topItems', //TODO: rename
            chlk.models.id.GradeLevelId, 'selectedItemId',
            Boolean, 'disabled',
            String, 'selectedIds',

            [[ArrayOf(chlk.models.grading.GradeLevel), String]],
            function $(gradeLevels_, selectedGradeLevelsIds_){
                BASE();
                if(gradeLevels_)
                    this.setTopItems(gradeLevels_);
                this.setSelectedIds(selectedGradeLevelsIds_ || '');
            }
        ]);
});
