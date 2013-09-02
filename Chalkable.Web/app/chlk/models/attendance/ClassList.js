REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceTypesValue*/
    ENUM('AttendanceTypesValue', {
        PRESENT: 2,
        EXCUSED: 4,
        ABSENT: 8,
        LATE: 16,
        NA: 1
    });

    /** @class chlk.models.attendance.ClassList*/
    CLASS(
        'ClassList', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.attendance.ClassAttendance), 'items',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            chlk.models.common.ChlkDate, 'date',

            Boolean, 'byLastName',

            [[chlk.models.class.ClassesForTopBar, chlk.models.id.ClassId, ArrayOf(chlk.models.attendance.ClassAttendance), chlk.models.common.ChlkDate,
                Boolean,  ArrayOf(chlk.models.attendance.AttendanceReason)]],
            function $(topData_, selectedId_, items_, date_, byLastName_, reasons_){
                BASE(topData_, selectedId_);
                if(items_)
                    this.setItems(items_);
                if(date_)
                    this.setDate(date_);
                if(reasons_)
                    this.setReasons(reasons_);
                if(byLastName_)
                    this.setByLastName(byLastName_);
            }
        ]);
});
