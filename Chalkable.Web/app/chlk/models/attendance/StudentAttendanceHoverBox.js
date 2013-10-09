REQUIRE('chlk.models.common.HoverBox');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBoxItem');

NAMESPACE('chlk.models.attendance', function(){
    "use strict";

    /**@class chlk.models.attendance.StudentAttendanceHoverBox*/
    CLASS('StudentAttendanceHoverBox', EXTENDS(chlk.models.common.HoverBox),[
        ArrayOf(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'hover'
    ]);
});
