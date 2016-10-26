REQUIRE('chlk.models.reports.BaseAdminReportViewData');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.LunchCountOrderBy*/
    ENUM('LunchCountOrderBy',{
        BY_CLASS: 0,
        MEAL_TYPE: 1,
        STUDENT: 2
    });

    /** @class chlk.models.reports.LunchCountAdditionalOptions*/
    ENUM('LunchCountAdditionalOptions',{
        GROUP_TOTALS: 0,
        GRAND_TOTALS: 1,
        STUDENTS_ONLY: 2,
        SUMMARY_ONLY: 3
    });


    var SJX = ria.serialize.SJX;

    /** @class chlk.models.reports.SubmitLunchCountViewData*/

    CLASS('SubmitLunchCountViewData', EXTENDS(chlk.models.reports.BaseAdminReportViewData), IMPLEMENTS(ria.serialize.IDeserializable), [
        String, 'includeOptions',
        chlk.models.reports.LunchCountOrderBy, 'orderBy',
        chlk.models.common.ChlkDate, 'startDate',
        chlk.models.common.ChlkDate, 'endDate',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.includeOptions = SJX.fromValue(raw.includeOptions, String);
            this.orderBy = SJX.fromValue(raw.orderBy, chlk.models.reports.LunchCountOrderBy);
            this.startDate = SJX.fromDeserializable(raw.startDate, chlk.models.common.ChlkDate);
            this.endDate = SJX.fromDeserializable(raw.endDate, chlk.models.common.ChlkDate);
        },

        [[Boolean, Number]],
        function $(ableDownload_, defaultIdToPrint_){
            BASE(ableDownload_, defaultIdToPrint_);
        }
    ]);
});
