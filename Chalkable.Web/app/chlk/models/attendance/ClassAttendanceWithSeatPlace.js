REQUIRE('chlk.models.attendance.ClassAttendance');
//REQUIRE('chlk.converters.attendance.AttendanceLevelToTypeConverter');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.ClassAttendanceWithSeatPlace*/
    CLASS(
        'ClassAttendanceWithSeatPlace', [
            chlk.models.attendance.ClassAttendance, 'info',

            Number, 'index',

            Number, 'column',

            Number, 'row',

            function $(row_, column_, index_){
                BASE();
                if(column_)
                    this.setColumn(column_);
                if(row_)
                    this.setRow(row_);
                if(index_)
                    this.setIndex(index_);
            }
        ]);
});
