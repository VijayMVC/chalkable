REQUIRE('chlk.models.common.AttendanceHoverBoxItem');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.AttendanceHoverBox*/
    CLASS(
        'AttendanceHoverBox', EXTENDS(chlk.models.common.HoverBox), [
            //ArrayOf(chlk.models.common.AttendanceHoverBoxItem), 'hover'
        ]);
});
