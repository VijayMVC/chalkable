REQUIRE('chlk.models.classes.ClassesForTopBar');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PageWithClasses*/
    CLASS(
        'PageWithClasses', [
            chlk.models.classes.ClassesForTopBar, 'topData', //todo: rename

            Number, 'selectedTypeId',

            chlk.models.id.ClassId, function getSelectedItemId(){return this.getTopData().getSelectedItemId(); },

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId]],
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
