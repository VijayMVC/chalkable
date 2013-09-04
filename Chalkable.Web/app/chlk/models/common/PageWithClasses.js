REQUIRE('chlk.models.class.ClassesForTopBar');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PageWithClasses*/
    CLASS(
        'PageWithClasses', [
            chlk.models.class.ClassesForTopBar, 'topData',
            Number, 'selectedTypeId',

            [[chlk.models.class.ClassesForTopBar, chlk.models.id.ClassId]],
            function $(topData_, selectedId_){
                BASE();
                if(topData_)
                    if(selectedId_)
                        topData_.setSelectedItemId(selectedId_);
                    this.setTopData(topData_);
            }
        ]);
});
