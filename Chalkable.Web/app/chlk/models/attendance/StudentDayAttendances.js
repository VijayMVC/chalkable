REQUIRE('chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentDayAttendances*/
    CLASS(
        'StudentDayAttendances', EXTENDS(chlk.models.Popup), [
            chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem, 'item',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            Boolean, 'canRePost',

            Boolean, 'canSetAttendance',

            Boolean, 'canChangeReasons',

            String, 'controller',

            String, 'action',

            String, 'params',

            [[ria.dom.Dom, chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem, ArrayOf(chlk.models.attendance.AttendanceReason),
                Boolean, Boolean, Boolean, String, String, String]],
            function $(target_, item_, reasons_, canRePost_, canSetAttendance_, canChangeReasons_, controller_, action_, params_){
                BASE(target_);
                if(item_)
                    this.setItem(item_);
                if(reasons_)
                    this.setReasons(reasons_);
                if(canRePost_)
                    this.setCanRePost(canRePost_);
                if(canSetAttendance_)
                    this.setCanSetAttendance(canSetAttendance_);
                if(canChangeReasons_)
                    this.setCanChangeReasons(canChangeReasons_);
                if(controller_)
                    this.setController(controller_);
                if(action_)
                    this.setAction(action_);
                if(params_)
                    this.setParams(params_);
            }


        ]);
});
