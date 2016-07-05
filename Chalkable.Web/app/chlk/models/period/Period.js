REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.PeriodId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.common.ChlkTime');
REQUIRE('chlk.models.id.DepartmentId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.period', function () {
    "use strict";


    var SJX = ria.serialize.SJX;

    /** @class chlk.models.period.Period*/
    CLASS(
        'Period', IMPLEMENTS(ria.serialize.IDeserializable), [


            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.PeriodId);
                this.name = SJX.fromValue(raw.name, String);
                this.startTime = SJX.fromDeserializable(raw.starttime, chlk.models.common.ChlkTime);
                this.endTime = SJX.fromDeserializable(raw.endtime, chlk.models.common.ChlkTime);
                this.order = SJX.fromValue(raw.order, Number);
                this.markingPeriodId = SJX.fromValue(raw.markingperiodid, chlk.models.id.MarkingPeriodId);
                this.roomId = SJX.fromValue(raw.roomid, Number);
                this.roomNumber = SJX.fromValue(raw.roomnumber, String);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.classNumber = SJX.fromValue(raw.classnumber, String);
                this.className = SJX.fromValue(raw.classname, String);
                this.departmentId = SJX.fromValue(raw.departmentid, chlk.models.id.DepartmentId);
                this.teacherId = SJX.fromValue(raw.teacherid, chlk.models.id.SchoolPersonId);
                this.teacherDisplayName = SJX.fromValue(raw.teacherdisplayname, String);
                this.teaching = SJX.fromValue(raw.teaching, Boolean);
                this.currentSection = SJX.fromValue(raw.iscurrentsection, Boolean);
            },

            chlk.models.id.PeriodId, 'id',

            String, 'name',

            chlk.models.common.ChlkTime, 'startTime',

            chlk.models.common.ChlkTime, 'endTime',

            Number, 'order',

            chlk.models.id.MarkingPeriodId, 'markingPeriodId',

            Number, 'roomId',

            String, 'roomNumber',

            chlk.models.id.ClassId, 'classId',

            String, 'classNumber',

            String, 'className',

            chlk.models.id.DepartmentId, 'departmentId',

            chlk.models.id.SchoolPersonId, 'teacherId',

            Boolean, 'teaching',

            String, 'teacherDisplayName',

            Boolean, 'currentSection',

            String, function getSerialOrder(){
                var order = this.getOrder();
                return order && getSerial(order);
            },

            [[String, Boolean]],
            String, function displayPeriodTimeRange(separatorBetweenTimes_, isRegularTime_){
                if(!this.getStartTime() || !this.getEndTime())
                    return '';
                separatorBetweenTimes_ = separatorBetweenTimes_ || ' - ';
                return this.getStartTime().format(isRegularTime_) + separatorBetweenTimes_
                    + this.getEndTime().format(isRegularTime_);
            },

            READONLY, String, 'fullClassName',
            String, function getFullClassName(){
                var classNumber = this.getClassNumber();
                if(classNumber) return classNumber + " " + this.getClassName();
                return this.getClassName();
            }
        ]);
});
