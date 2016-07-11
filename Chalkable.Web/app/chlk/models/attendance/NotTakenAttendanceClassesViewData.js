REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.NotTakenAttendanceClassesViewData*/
    CLASS(
        'NotTakenAttendanceClassesViewData', [
            function $(items_){
                BASE();
                if(items_)
                    this.items = items_;
            },

            ArrayOf(chlk.models.common.NameId), 'items'
        ]);
});
