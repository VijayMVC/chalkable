REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.attendance.StudentAttendanceHoverBox');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.attendance', function(){
   "use strict";

    /**@class chlk.models.attendance.StudentAttendanceSummary*/
    CLASS('StudentAttendanceSummary', EXTENDS(chlk.models.people.ShortUserInfo),[

        [ria.serialize.SerializeProperty('markingperiod')],
        chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',

        [ria.serialize.SerializeProperty('absentsection')],
        chlk.models.attendance.StudentAttendanceHoverBox, 'absentSection',

        [ria.serialize.SerializeProperty('latesection')],
        chlk.models.attendance.StudentAttendanceHoverBox, 'lateSection',

        [ria.serialize.SerializeProperty('excusedsection')],
        chlk.models.attendance.StudentAttendanceHoverBox, 'excusedSection',

        [ria.serialize.SerializeProperty('presentsection')],
        chlk.models.attendance.StudentAttendanceHoverBox, 'presentSection'
    ]);
});