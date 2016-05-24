REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.profile.StandardizedTestViewData');
REQUIRE('chlk.models.profile.ClassDistributionSectionViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.ClassPanoramaViewData*/
    CLASS(
        UNSAFE, 'ClassPanoramaViewData', EXTENDS(chlk.models.classes.Class), [
            chlk.models.profile.PanoramaSettingsViewData, 'filterSettings',
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',
            chlk.models.profile.ClassDistributionSectionViewData, 'classDistributionSection',

            VOID, function deserialize(raw) {
                this.schoolYearIds = SJX.fromArrayOfValues(raw.schoolyearids, chlk.models.id.SchoolYearId);
                this.standardizedTestFilters = SJX.fromArrayOfDeserializables(raw.standardizedtestfilters, chlk.models.profile.StandardizedTestViewData);
                this.classDistributionSection = SJX.fromDeserializable(raw.classdistributionsection, chlk.models.profile.ClassDistributionSectionViewData)
            }
        ]);
});
