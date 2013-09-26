REQUIRE('chlk.models.grading.GradeLevelForTopBar');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradeLevelsForTopBar*/


    //TODO: refactor
    CLASS(
        'GradeLevelsForTopBar', [
            ArrayOf(chlk.models.grading.GradeLevelForTopBar), 'topItems', //TODO: rename
            chlk.models.id.GradeLevelId, 'selectedItemId',
            Boolean, 'disabled',
            Array, 'selectedIds',

            [[ArrayOf(chlk.models.grading.GradeLevelForTopBar), String]],
            function $(gradeLevels_, selectedGradeLevelsIds_){
                BASE();
                if(gradeLevels_)
                    this.setTopItems(gradeLevels_);
                var slGlIdsArray = selectedGradeLevelsIds_ && selectedGradeLevelsIds_ != ''
                                     ? selectedGradeLevelsIds_.split(',') : [];
                this.setSelectedIds(slGlIdsArray);
            }
        ]);
});
