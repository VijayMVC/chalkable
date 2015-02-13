REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.attendance.ClassAttendanceWithSeatPlace');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');


NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.SeatingChart*/
    CLASS(
        'SeatingChart', EXTENDS(chlk.models.common.PageWithClasses), IMPLEMENTS(ria.serialize.IDeserializable), [
            Number, 'columns',

            Number, 'rows',

            Boolean, 'ablePost',

            Boolean, 'ableRePost',

            [ria.serialize.SerializeProperty('isscheduled')],
            Boolean, 'scheduled',

            chlk.models.common.ChlkDate, 'date',

            Boolean, 'ableChangeReasons',

            [ria.serialize.SerializeProperty('notseatingstudents')],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'notSeatingStudents',

            [ria.serialize.SerializeProperty('seatinglist')],
            ArrayOf(ArrayOf(chlk.models.attendance.ClassAttendanceWithSeatPlace)), 'seatingList',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',


            VOID, function deserialize(raw){
//                this.setId(new chlk.models.apps.AppPermissionTypeEnum(raw.type));
//                this.setName(raw.name);
                var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                this.setRows(raw.rows);
                this.setColumns(raw.columns);
                this.setScheduled(raw.isscheduled);
                this.setDate(new chlk.models.common.ChlkSchoolYearDate(raw.date));
                var seatingList = [];
                for(var i = 0; i < raw.seatinglist.length; i++){
                    var items = [];
                    for(var j = 0; j < raw.seatinglist[i].length; j++){
                        var item = new chlk.models.attendance.ClassAttendanceWithSeatPlace();
                        item.deserialize(raw.seatinglist[i][j]);
                        items.push(item);
                    }
                    seatingList.push(items);
                }
                this.setSeatingList(seatingList);
                if(raw.reasons){
                    this.setReasons(serializer.deserialize(raw.reasons, ArrayOf(chlk.models.attendance.AttendanceReason)));
                }
                if(raw.notseatingstudents){
                    this.setNotSeatingStudents(serializer.deserialize(raw.notseatingstudents, ArrayOf(chlk.models.attendance.ClassAttendance)));
                }
            },

            Boolean, function hasSeatingStudents(){
                var p = false;
                this.getSeatingList().forEach(function(items){
                    if(!p)
                        items.forEach(function(item){
                            if(!p && item.getInfo())
                                p = true;
                        });
                });
                return p;
            },

            Boolean, function isPosted(){
                var posted1, posted2, len = 0;

                var items = this.getNotSeatingStudents();
                posted1 =  items && items.length > 0
                    && items.filter(function(item){return item.isPosted()}).length > 0;

                var seatingList = this.getSeatingList();
                seatingList && seatingList.forEach(function(items){
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
