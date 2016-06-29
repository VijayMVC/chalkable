REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.profile.StandardizedTestViewData');
REQUIRE('chlk.models.profile.PanoramaSettingsViewData');
REQUIRE('chlk.models.profile.StandardizedTestStatsViewData');
REQUIRE('chlk.models.schoolYear.Year');
REQUIRE('chlk.models.common.ChartDateItem');

NAMESPACE('chlk.models.panorama', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.panorama.StudentPanoramaViewData*/
    CLASS(
        UNSAFE, 'StudentPanoramaViewData', EXTENDS(chlk.models.classes.Class), [
            chlk.models.profile.PanoramaSettingsViewData, 'filterSettings',
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStats',
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',
            ArrayOf(chlk.models.common.ChartDateItem), 'dailyDisciplines',
            ArrayOf(chlk.models.common.ChartDateItem), 'dailyAttendances',
            ArrayOf(chlk.models.panorama.StudentDisciplineStatViewData), 'studentDisciplineStats',


            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.filterSettings = SJX.fromDeserializable(raw.filtersettings, chlk.models.profile.PanoramaSettingsViewData);
                this.standardizedTests = SJX.fromArrayOfDeserializables(raw.standardizedtests, chlk.models.profile.StandardizedTestViewData);
                this.standardizedTestsStats = SJX.fromArrayOfDeserializables(raw.standardizedtestsstats, chlk.models.profile.StandardizedTestStatsViewData);
                this.dailyDisciplines = SJX.fromArrayOfDeserializables(raw.dailydisciplinestats, chlk.models.common.ChartDateItem);
            }
        ]);

    /** @class chlk.models.panorama.StudentDisciplineStatViewData*/
    CLASS(
        UNSAFE, 'StudentDisciplineStatViewData', [
            chlk.models.common.ChlkDate, 'occurrenceDate',
            String, 'infractionName',
            String, 'infractionStateCode',
            Number, 'demeritsEarned',
            Boolean, 'primary',
            String, 'dispositionName',
            String, 'dispositionNote',
            chlk.models.common.ChlkDate, 'dispositionStartDate',

            VOID, function deserialize(raw) {
                this.occurrenceDate = SJX.fromDeserializable(raw.occurrencedate, chlk.models.common.ChlkDate);
                this.infractionName = SJX.fromValue(raw.infractionname, String);
                this.infractionStateCode = SJX.fromValue(raw.infractionstatecode, String);
                this.demeritsEarned = SJX.fromValue(raw.demeritsearned, Number);
                this.primary = SJX.fromValue(raw.isprimary, Boolean);
                this.dispositionName = SJX.fromValue(raw.dispositionname, String);
                this.dispositionNote = SJX.fromValue(raw.dispositionnote, String);
                this.dispositionStartDate = SJX.fromDeserializable(raw.dispositionstartdate, chlk.models.common.ChlkDate);
            }
        ]);

    /** @class chlk.models.panorama.StudentAbsenceStatViewData*/
    CLASS(
        UNSAFE, 'StudentAbsenceStatViewData', [
            chlk.models.common.ChlkDate, 'date',
            String, 'absenceReasonName',
            String, 'absenceLevel',
            String, 'absenceCategory',
            ArrayOf(String), 'periods',

            VOID, function deserialize(raw) {
                this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
                this.absenceReasonName = SJX.fromValue(raw.absencereasonname, String);
                this.absenceLevel = SJX.fromValue(raw.absencelevel, String);
                this.absenceCategory = SJX.fromValue(raw.absencecategory, String);
                this.periods = SJX.fromArrayOfValues(raw.periods, String);
            }
        ]);
});
