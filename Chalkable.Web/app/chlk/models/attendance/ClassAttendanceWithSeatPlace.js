REQUIRE('chlk.models.attendance.ClassAttendance');
//REQUIRE('chlk.converters.attendance.AttendanceLevelToTypeConverter');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.ClassAttendanceWithSeatPlace*/
    CLASS(
        UNSAFE, 'ClassAttendanceWithSeatPlace', IMPLEMENTS(ria.serialize.IDeserializable), [

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
            },

            VOID, function deserialize(raw){
                this.setIndex(raw.index);
                this.setColumn(raw.column);
                this.setRow(raw.row);
                if(raw.info){
                    var info = new chlk.models.attendance.ClassAttendance();
                    info.setId(new chlk.models.id.ClassAttendanceId(raw.info.id));
                    if (raw.info.type) info.setType(raw.info.type);
                    info.setLevel(raw.info.level);
                    var student = new chlk.models.people.User();
                    student.setId(new chlk.models.id.SchoolPersonId(raw.info.student.id));
                    student.setDisplayName(raw.info.student.displayname);
                    student.setFirstName(raw.info.student.firstname);
                    student.setFullName(raw.info.student.fullname);
                    student.setGender(raw.info.student.gender);
                    student.setLastName(raw.info.student.lastname);

                    if(raw.info.student.role){
                        var objRole =  raw.info.student.role;
                        var role = new chlk.models.people.Role.$create(objRole.id, objRole.name, objRole.namelowered, objRole.description);
                        student.setRole(role);
                    }
                    info.setStudent(student);
                    info.setAbsentPreviousDay(raw.info.absentpreviousday);
                    info.setAttendanceReasonId(new chlk.models.id.AttendanceReasonId(raw.info.attendancereasonid));
                    if (raw.info.attendancereason){
                        var attReason = new chlk.models.attendance.AttendanceReason();
                        attReason.deserialize(raw.info.attendancereason);
                        info.setAttendanceReason(attReason);
                    }
//                    var reasons = [];
//                    for(var i = 0; i < raw.info.reasons.length; i++){
//                        if(raw.info.reasons[i]){
//                            var item = new chlk.models.attendance.AttendanceReason();
//                            item.deserialize(raw.info.reasons[i]);
//                            reasons.push(item);
//                        }
//                    }
//                    info.setReasons(reasons);
                    info.setPosted(raw.info.isposted);
                    info.setClassName(raw.info.classname);
                    info.setClassId(new chlk.models.id.ClassId(raw.info.classid));
                    this.setInfo(info);
                }
            }
        ]);
});
