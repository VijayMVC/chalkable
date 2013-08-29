REQUIRE('chlk.models.class.Class');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassesForTopBar*/
    CLASS(
        'ClassesForTopBar', [
            ArrayOf(chlk.models.class.Class), 'topItems',
            chlk.models.id.ClassId, 'selectedItemId',
            Boolean, 'disabled',

            [[ArrayOf(chlk.models.class.Class)]],
            function $(classes_){
                BASE();
                if(classes_)
                    this.setTopItems(classes_);
            }
        ]);
});
