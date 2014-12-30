REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.attendance.StudentAttendanceHoverBoxItem*/
    CLASS(
        UNSAFE, FINAL, 'StudentAttendanceHoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.value = SJX.fromValue(raw.attendancecount, Number);
                this.className = SJX.fromValue(raw.classname, String);
            },

            Number, 'value',
            String, 'className'
        ]);
});
