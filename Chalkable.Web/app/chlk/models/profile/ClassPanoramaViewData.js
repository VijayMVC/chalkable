REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.profile.StandardizedTestViewData');
REQUIRE('chlk.models.profile.ClassDistributionSectionViewData');
REQUIRE('chlk.models.profile.PanoramaSettingsViewData');
REQUIRE('chlk.models.profile.StandardizedTestStatsViewData');
REQUIRE('chlk.models.panorama.StudentStandardizedTestStats');
REQUIRE('chlk.models.schoolYear.Year');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.ClassPanoramaViewData*/
    CLASS(
        UNSAFE, 'ClassPanoramaViewData', EXTENDS(chlk.models.classes.Class), [
            chlk.models.profile.PanoramaSettingsViewData, 'filterSettings',
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',
            chlk.models.profile.ClassDistributionSectionViewData, 'classDistributionSection',
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStatsByClass',
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'selectStandardizedTestsStats',
            ArrayOf(chlk.models.schoolYear.Year), 'schoolYears',
            ArrayOf(chlk.models.panorama.StudentStandardizedTestStats), 'students',
            Boolean, 'showFilters',

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.filterSettings = SJX.fromDeserializable(raw.filtersettings, chlk.models.profile.PanoramaSettingsViewData);
                this.standardizedTests = SJX.fromArrayOfDeserializables(raw.standardizedtests, chlk.models.profile.StandardizedTestViewData);
                this.classDistributionSection = SJX.fromDeserializable(raw.classdistributionsection, chlk.models.profile.ClassDistributionSectionViewData);
                this.standardizedTestsStatsByClass = SJX.fromArrayOfDeserializables(raw.standardizedtestsstatsbyclass, chlk.models.profile.StandardizedTestStatsViewData);
                this.selectStandardizedTestsStats = SJX.fromArrayOfDeserializables(raw.selectstandardizedtestsstats, chlk.models.profile.StandardizedTestStatsViewData);
                this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.panorama.StudentStandardizedTestStats);
            }
        ]);
});
