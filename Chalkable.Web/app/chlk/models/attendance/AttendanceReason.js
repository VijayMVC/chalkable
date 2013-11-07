REQUIRE('chlk.models.id.AttendanceReasonId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceReason*/
    CLASS(
        'AttendanceReason', [
            [ria.serialize.SerializeProperty('attendancetype')],
            Number, 'attendanceType',

            chlk.models.id.AttendanceReasonId, 'id',

            String, 'description',

            [[chlk.models.id.AttendanceReasonId, String]],
            function $(id_, description_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(description_)
                    this.setDescription(description_);
            }
        ]);
});
