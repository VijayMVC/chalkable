REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBoxItem');

NAMESPACE('chlk.models.attendance', function(){
   "use strict";

    /**@class chlk.models.attendance.StudentAttendanceSummary*/
    CLASS('StudentAttendanceSummary', EXTENDS(chlk.models.people.ShortUserInfo),[

        [ria.serialize.SerializeProperty('markingperiod')],
        chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',

        [ria.serialize.SerializeProperty('absentsection')],
        chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'absentSection',

        [ria.serialize.SerializeProperty('latesection')],
        chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'lateSection',

        [ria.serialize.SerializeProperty('excusedsection')],
        chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'excusedSection',

        [ria.serialize.SerializeProperty('presentsection')],
        chlk.models.common.HoverBox.OF(chlk.models.attendance.StudentAttendanceHoverBoxItem), 'presentSection'
    ]);
});