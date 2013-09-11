REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassesForTopBar*/
    CLASS(
        'ClassesForTopBar', [
            ArrayOf(chlk.models.classes.Class), 'topItems', //todo: rename
            chlk.models.id.ClassId, 'selectedItemId',
            Boolean, 'disabled',

            [[ArrayOf(chlk.models.classes.Class)]],
            function $(classes_){
                BASE();
                if(classes_)
                    this.setTopItems(classes_);
            }
        ]);
});
