REQUIRE('chlk.models.grading.GradeLevelForTopBar');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradeLevelsForTopBar*/
    CLASS(
        'GradeLevelsForTopBar', [
            ArrayOf(chlk.models.grading.GradeLevelForTopBar), 'topItems',
            chlk.models.id.GradeLevelId, 'selectedItemId',
            Boolean, 'disabled',
            Array, 'selectedIds',

            [[ArrayOf(chlk.models.grading.GradeLevelForTopBar)]],
            function $(gradeLevels_){
                BASE();
                if(gradeLevels_)
                    this.setTopItems(gradeLevels_);
            }
        ]);
});
