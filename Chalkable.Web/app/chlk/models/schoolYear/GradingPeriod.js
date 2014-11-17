REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.schoolYear.GradingPeriod*/
    CLASS(
        UNSAFE, 'GradingPeriod', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.GradingPeriodId);
                this.name = SJX.fromValue(raw.name, String);
                this.description = SJX.fromValue(raw.description, String);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.ablePostGradeBook = SJX.fromValue(raw.allowgradeposting, String);
            },

            READONLY, chlk.models.id.GradingPeriodId, 'id',
            READONLY, String, 'name',
            READONLY, String, 'description',
            READONLY, chlk.models.common.ChlkDate, 'startDate',
            READONLY, chlk.models.common.ChlkDate, 'endDate',
            READONLY, Boolean, 'ablePostGradeBook',

            String, function getFinalTitleText(){
                var res = this.getName() + ': ' + this.getStartDate().format('m/d/y - ')
                    + this.getEndDate().format('m/d/y');
                return res;
            }
        ]);
});
