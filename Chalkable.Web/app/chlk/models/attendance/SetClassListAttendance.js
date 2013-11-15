REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";
    /**@class chlk.models.attendance.SetClassAttendanceItem*/
    CLASS('SetClassAttendanceItem', [

        chlk.models.id.SchoolPersonId, 'personId',
        String, 'level',
        chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

        Number, 'type',
        [[Number]],
        VOID, function setType(type){
            if(type && type > 0){
                this._type = type;
                var level = this._mapper.mapBack(new chlk.models.attendance.AttendanceTypeEnum(type));
                this.setLevel(level);
            }
        },
        Number, function getType(){ return this._type; },

        function $(){
            BASE();
            this._type = null;
            this._mapper = new chlk.models.attendance.AttendanceTypeMapper();
        },

        Object, function getPostData(){
            return{
                personid: this.getPersonId() && this.getPersonId().valueOf(),
                level: this.getLevel(),
                attendancereasonid: this.getAttendanceReasonId() && this.getAttendanceReasonId().valueOf()
            }
        }

    ]);

    /** @class chlk.models.attendance.SetClassListAttendance*/
    CLASS('SetClassListAttendance',[
        [ria.serialize.SerializeProperty('classid')],
        chlk.models.id.ClassId, 'classId',
        chlk.models.common.ChlkDate, 'date',
        ArrayOf(chlk.models.attendance.SetClassAttendanceItem), 'items',

        String, 'attendancesJson',

        [[String]],
        VOID, function setAttendancesJson(attendancesJson){
            var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
            var attendancesObj = attendancesJson ? JSON.parse(attendancesJson) : null;
            this.setItems(serializer.deserialize(attendancesObj, ArrayOf(chlk.models.attendance.SetClassAttendanceItem)));
        },

        Array, function getPostItems(){
            return this.getItems() && this.getItems().map(function(item){
                console.log(item.getPostData());
                return item.getPostData();
            });
        },
        Object, function getPostData(){
            var res ={
                classid: this.getClassId() && this.getClassId().valueOf(),
                date: this.getDate().toStandardFormat(),
                items: this.getPostItems()
            };
            console.log(res);
            return res;
        }
    ]);
});
