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
        Boolean, 'defaultReason',

        [[chlk.models.id.AttendanceLevelReasonId, chlk.models.id.AttendanceReasonId, String, Boolean]],
        function $(id_, attendanceReasonId_, level_, defaultReason_){
            BASE();
            this.id = id_;
            this.attendanceReasonId = attendanceReasonId_;
            this.level = level_;
            this.defaultReason = defaultReason_;
        }
    ]);

    /** @class chlk.models.attendance.AttendanceReason*/
    CLASS(
        'AttendanceReason', IMPLEMENTS(ria.serialize.IDeserializable), [

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
                    this.id = id_;
                if(name_)
                    this.name = name_;
                if(description_)
                    this._description = description_;
                if(attLevelReasons_)
                    this.setAttendanceLevelReasons(attLevelReasons_);
            },

            [[String]],
            function getAttendanceLevelReason_(level){
                var attLevelReasons = this.getAttendanceLevelReasons(),res,that = this;
                if(!attLevelReasons){
                    var reason = window.attendanceReasons.filter(function(item){
                        return that.getId().valueOf() == item.id;
                    })[0];
                    if(!reason)
                        return null;
                    attLevelReasons = reason.attendancelevelreason;
                    res = attLevelReasons.filter(function(item){return item.level == level;});
                }else{
                    res = attLevelReasons.filter(function(item){return item.getLevel() == level;});
                }
                return res.length > 0 ? res[0] : null;
            },

            [[String]],
            Boolean, function hasLevel(level){
               return this.getAttendanceLevelReason_(level) != null;
            },

            function getLevel(type){
                if(type == chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf())
                    return 'T';
                if(this.hasLevel('A')) return 'A';
                if(this.hasLevel('AO')) return 'AO';
                if(this.hasLevel('H')) return 'H';
                if(this.hasLevel('IO')) return 'IO';
            },

            [[String]],
            Boolean, function isDefaultReason(level){
                var reason = this.getAttendanceLevelReason_(level);
                return reason != null && reason.isDefaultReason();
            },

            VOID, function deserialize(raw){
                this.setId(new chlk.models.id.AttendanceReasonId(raw.id));
                this.setName(raw.name);
                this.setDescription(raw.description);
                this.setCategory(raw.category);
                if(raw.attendancelevelreason){
                    var attReasonsLevels = [];
                    for(var i = 0; i < raw.attendancelevelreason.length; i++){
                        attReasonsLevels.push(new chlk.models.attendance.AttendanceLevelReason(
                            new chlk.models.id.AttendanceLevelReasonId(raw.attendancelevelreason[i].id),
                            new chlk.models.id.AttendanceReasonId(raw.attendancelevelreason[i].attendancereasonid),
                            raw.attendancelevelreason[i].level,
                            raw.attendancelevelreason[i].isdefault
                        ));
                    }
                    this.setAttendanceLevelReasons(attReasonsLevels);
                }

            }
    ]);
});
