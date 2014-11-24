REQUIRE('chlk.models.id.PeriodId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.common.ChlkTime');
REQUIRE('chlk.models.id.DepartmentId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.period', function () {
    "use strict";

    /** @class chlk.models.period.Period*/
    CLASS(
        'Period', [
            chlk.models.id.PeriodId, 'id',

            [ria.serialize.SerializeProperty('starttime')],
            chlk.models.common.ChlkTime, 'startTime',

            [ria.serialize.SerializeProperty('endtime')],
            chlk.models.common.ChlkTime, 'endTime',

            Number, 'order',

            [ria.serialize.SerializeProperty('markingperiodid')],
            chlk.models.id.MarkingPeriodId, 'markingPeriodId',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber',

            [ria.serialize.SerializeProperty('classnumber')],
            String, 'classNumber',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className',

            [ria.serialize.SerializeProperty('roomid')],
            Number, 'roomId',

            [ria.serialize.SerializeProperty('departmentid')],
            chlk.models.id.DepartmentId, 'departmentId',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

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
