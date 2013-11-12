REQUIRE('chlk.models.id.AttendanceReasonId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceLevelReason*/
    CLASS('AttendanceLevelReason',[
        chlk.models.id.AttendanceLevelReasonId, 'id',

        [ria.serialize.SerializeProperty('attendancereasonid')],
        chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

        String, 'level',

        [ria.serialize.SerializeProperty('isdefault')],
        Boolean, 'defaultReason'
    ]);

    /** @class chlk.models.attendance.AttendanceReason*/
    CLASS(
        'AttendanceReason', [
//            [ria.serialize.SerializeProperty('attendancetype')],
//            Number, 'attendanceType',

            chlk.models.id.AttendanceReasonId, 'id',
            String, 'name',
            String, 'description',
            String, 'category',

            [ria.serialize.SerializeProperty('attendancelevelreason')],
            ArrayOf(chlk.models.attendance.AttendanceLevelReason), 'attendanceLevelReasons',

            [[chlk.models.id.AttendanceReasonId, String, ArrayOf(chlk.models.attendance.AttendanceLevelReason)]],
            function $(id_, description_, attLevelReasons_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(description_)
                    this.setDescription(description_);
                if(attLevelReasons_)
                    this.setAttendanceLevelReasons(attLevelReasons_);
            },

            [[String]],
            Boolean, function hasLevel(level){
                var attLevelReasons = this.getAttendanceLevelReasons();
                return attLevelReasons && attLevelReasons.filter(function(item){return this.getLevel() == level;}).length > 0;
            }
    ]);
});
