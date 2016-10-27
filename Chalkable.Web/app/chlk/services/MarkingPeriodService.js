REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.id.SchoolYearId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.MarkingPeriodService */
    CLASS(
        'MarkingPeriodService', EXTENDS(chlk.services.BaseService), [

            READONLY, chlk.models.schoolYear.MarkingPeriod, 'currentMarkingPeriod',

            ArrayOf(chlk.models.schoolYear.MarkingPeriod), function getMarkingPeriodsSync() {
                return this.context.getSession().get(ChlkSessionConstants.MARKING_PERIODS, []);
            },

            chlk.models.schoolYear.MarkingPeriod, function getCurrentMarkingPeriod() {
                return this.context.getSession().get(ChlkSessionConstants.MARKING_PERIOD, []);
            },

            [[chlk.models.id.SchoolYearId]],
            ria.async.Future, function list(schoolYearId) {
                return this.get('MarkingPeriod/List.json', ArrayOf(chlk.models.schoolYear.MarkingPeriod), {
                    schoolYearId: schoolYearId.valueOf()
                });
            }
        ])
});