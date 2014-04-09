REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.attendance.ClassAttendanceWithSeatPlace');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.SeatingChart*/
    CLASS(
        'SeatingChart', EXTENDS(chlk.models.common.PageWithClasses), [
            Number, 'columns',

            Number, 'rows',

            Boolean, 'ablePost',

            Boolean, 'ableRePost',

            [ria.serialize.SerializeProperty('isscheduled')],
            Boolean, 'scheduled',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('notseatingstudents')],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'notSeatingStudents',

            [ria.serialize.SerializeProperty('seatinglist')],
            ArrayOf(ArrayOf(chlk.models.attendance.ClassAttendanceWithSeatPlace)), 'seatingList',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            Boolean, function isPosted(){
                var posted1, posted2, len = 0;

                var items = this.getNotSeatingStudents();
                posted1 =  items && items.length > 0
                    && items.filter(function(item){return item.isPosted()}).length > 0;

                this.getSeatingList().forEach(function(items){
                    items.forEach(function(item){
                        if(item.getInfo() && item.getInfo().isPosted())
                            len++;
                    });
                });
                posted2 = len > 0;

                return posted1 || posted2;
            },

            Boolean, function canPost(){
                return this.isAblePost()  && (!this.isPosted() || !!this.isAbleRePost());
            }
        ]);
});
