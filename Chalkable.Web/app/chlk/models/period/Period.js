REQUIRE('chlk.models.id.PeriodId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.common.ChlkTime');
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

            String, function getSerialOrder(){
                var order = this.getOrder();
                return order && getSerial(order);
            },
            [[String, Boolean]],
            String, function displayPeriodTimeRange(separatorBetweenTimes_, isRegularTime_){
                separatorBetweenTimes_ = separatorBetweenTimes_ || ' - ';
                return this.getStartTime().format(isRegularTime_) + separatorBetweenTimes_
                    + this.getEndTime().format(isRegularTime_);
            }
        ]);
});
