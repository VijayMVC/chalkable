REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.profile.StandardizedTestFilterViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.PanoramaSettingsViewData*/
    CLASS(
        UNSAFE, 'PanoramaSettingsViewData', [
            ArrayOf(chlk.models.id.SchoolYearId), 'schoolYearIds',
            ArrayOf(chlk.models.profile.StandardizedTestFilterViewData), 'standardizedTestFilters',

            VOID, function deserialize(raw) {
                this.schoolYearIds = SJX.fromArrayOfValues(raw.schoolyearids, chlk.models.id.SchoolYearId);
                this.standardizedTestFilters = SJX.fromArrayOfDeserializables(raw.standardizedtestfilters, chlk.models.profile.StandardizedTestFilterViewData);
            }
        ]);
});
