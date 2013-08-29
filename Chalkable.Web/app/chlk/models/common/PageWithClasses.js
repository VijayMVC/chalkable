REQUIRE('chlk.models.class.ClassesForTopBar');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PageWithClasses*/
    CLASS(
        'PageWithClasses', [
            chlk.models.class.ClassesForTopBar, 'topData',
            Number, 'selectedTypeId',

            [[chlk.models.class.ClassesForTopBar]],
            function $(topData_){
                BASE();
                if(topData_)
                    this.setTopData(topData_);
            }
        ]);
});
