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
            String, function getDescription(){ return this._description || this.getName(); },
            [[String]],
            VOID, function setDescription(description){ this._description = description; },

            String, 'category',

            [ria.serialize.SerializeProperty('attendancelevelreason')],
            ArrayOf(chlk.models.attendance.AttendanceLevelReason), 'attendanceLevelReasons',

            [[chlk.models.id.AttendanceReasonId, String, String, ArrayOf(chlk.models.attendance.AttendanceLevelReason)]],
            function $(id_, name_, description_, attLevelReasons_){
                BASE();
                this._description = null;
                if(id_)
                    this.setId(id_);
                if(name_)
                    this.setName(name_);
                if(description_)
                    this._description = description_;
                if(attLevelReasons_)
                    this.setAttendanceLevelReasons(attLevelReasons_);
            },

            [[String]],
            chlk.models.attendance.AttendanceLevelReason, function getAttendanceLevelReason_(level){
                var attLevelReasons = this.getAttendanceLevelReasons();
                if(!attLevelReasons) return null;
                var res = attLevelReasons.filter(function(item){return item.getLevel() == level;});
                return res.length > 0 ? res[0] : null;
            },

            [[String]],
            Boolean, function hasLevel(level){
               return this.getAttendanceLevelReason_(level) != null;
            },
            [[String]],
            Boolean, function isDefaultReason(level){
                var reason = this.getAttendanceLevelReason_(level);
                return reason != null && reason.isDefaultReason();
            }
    ]);
});
