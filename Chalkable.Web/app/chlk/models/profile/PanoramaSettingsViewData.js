REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.profile.StandardizedTestFilterViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.PanoramaSettingsViewData*/
    CLASS(
        UNSAFE, 'PanoramaSettingsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            ArrayOf(chlk.models.id.SchoolYearId), 'acadYears',
            ArrayOf(chlk.models.profile.StandardizedTestFilterViewData), 'standardizedTestFilters',

            VOID, function deserialize(raw) {
                this.acadYears = SJX.fromArrayOfValues(raw.acadyears, chlk.models.id.SchoolYearId);
                this.standardizedTestFilters = SJX.fromArrayOfDeserializables(raw.standardizedtestfilters, chlk.models.profile.StandardizedTestFilterViewData);
            }
        ]);
});
