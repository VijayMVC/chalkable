REQUIRE('chlk.models.grading.GradeLevelsForTopBar');
REQUIRE('chlk.models.id.GradeLevelId');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PageWithGrades*/
    CLASS(
        'PageWithGrades', [
            chlk.models.grading.GradeLevelsForTopBar, 'topData',  //todo: rename
            Number, 'selectedTypeId',
            Array, 'selectedIds',

            [[chlk.models.grading.GradeLevelForTopBar, chlk.models.id.GradeLevelId]],
            function $(topData_, selectedId_){
                BASE();
                if(topData_){
                    if(selectedId_)
                        topData_.setSelectedItemId(selectedId_);
                    this.setTopData(topData_);
                }
            }
        ]);
});
