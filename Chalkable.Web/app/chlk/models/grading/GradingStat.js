REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingStat*/
    CLASS(
        'GradingStat', [
            [ria.serialize.SerializeProperty('departmentid')],
            chlk.models.id.DepartmentId, 'departmentId',

            String, 'title',

            [ria.serialize.SerializeProperty('avgbygradelevels')],
            Number, 'avgByGradeLevels',

            [ria.serialize.SerializeProperty('fullavg')],
            Number, 'fullAvg'
        ]);
});
