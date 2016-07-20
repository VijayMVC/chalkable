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

    /** @class chlk.models.panorama.StudentAttendancesSortType*/
    ENUM('StudentAttendancesSortType', {
        DATE: 1,
        REASON: 2,
        LEVEL: 3,
        CATEGORY: 4,
        PERIODS: 5,
        NOTE: 6
    });

    /** @class chlk.models.panorama.StudentDisciplinesSortType*/
    ENUM('StudentDisciplinesSortType', {
        DATE: 1,
        INFRACTION: 2,
        CODE: 3,
        DEMERITS: 4,
        PRIMARY: 5,
        DISPOSITION_DATE: 6,
        DISPOSITION: 7,
        NOTE: 8
    });

    /** @class chlk.models.panorama.StudentDisciplineStatViewData*/
    CLASS(
        UNSAFE, 'StudentDisciplineStatViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
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
        UNSAFE, 'StudentAbsenceStatViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.common.ChlkDate, 'date',
            String, 'absenceReasonName',
            String, 'absenceLevel',
            String, 'absenceCategory',
            ArrayOf(String), 'periods',
            String, 'note',

            VOID, function deserialize(raw) {
                this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
                this.absenceReasonName = SJX.fromValue(raw.absencereasonname, String);
                this.absenceLevel = SJX.fromValue(raw.absencelevel, String);
                this.absenceCategory = SJX.fromValue(raw.absencecategory, String);
                this.periods = SJX.fromArrayOfValues(raw.periods, String);
            },

            function getPeriodsText(){
                var values = this.getPeriods().sort(), first, last, resArr = [], fIndex, fName, lName;

                if(values.length){

                    if(values.length == 1)
                        resArr.push(values[0]);

                    if(values.length == 2){
                        resArr.push(values[0]);
                        resArr.push(values[1]);
                    }

                    if(values.length > 2){
                        values.forEach(function(value, index){
                            if(!first && first !== 0){
                                first = value;
                                fIndex = index;
                                fName = values[index];
                            }
                            else
                            if(!last){
                                if((value - first) != (index - fIndex)){
                                    resArr.push(fName);
                                    first = value;
                                    fName = values[index];
                                    fIndex = index;
                                }else{
                                    last = value;
                                    lName = values[index];
                                }
                            }else{
                                if((value - first) != (index - fIndex)){
                                    if((last - first) == 1){
                                        resArr.push(fName);
                                        resArr.push(lName);
                                    }else{
                                        resArr.push(fName + '-' + lName);
                                    }
                                    fIndex = index;
                                    last = null;
                                    first = value;
                                    fName = values[index];
                                }else{
                                    last = value;
                                    lName = values[index];
                                }

                            }
                            if(index == values.length - 1){
                                if(!last)
                                    resArr.push(fName);
                                else
                                if((last - first) == 1){
                                    resArr.push(fName);
                                    resArr.push(lName);
                                }else{
                                    resArr.push(fName + '-' + lName);
                                }
                            }

                        });
                    }
                }

                return resArr.join(',');
            }
        ]);

    /** @class chlk.models.panorama.StudentPanoramaViewData*/
    CLASS(
        UNSAFE, 'StudentPanoramaViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.profile.PanoramaSettingsViewData, 'filterSettings',
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStats',
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',
            ArrayOf(chlk.models.common.ChartDateItem), 'dailyDisciplines',
            ArrayOf(chlk.models.common.ChartDateItem), 'attendancesStats',
            ArrayOf(chlk.models.panorama.StudentDisciplineStatViewData), 'studentDisciplineStats',
            ArrayOf(chlk.models.panorama.StudentAbsenceStatViewData), 'studentAbsenceStats',
            ArrayOf(chlk.models.schoolYear.Year), 'schoolYears',
            Boolean, 'showFilters',

            chlk.models.panorama.StudentAttendancesSortType, 'attendancesOrderBy',
            Boolean, 'attendancesDescending',

            chlk.models.panorama.StudentDisciplinesSortType, 'disciplinesOrderBy',
            Boolean, 'disciplinesDescending',

            VOID, function deserialize(raw) {
                this.filterSettings = SJX.fromDeserializable(raw.filtersettings, chlk.models.profile.PanoramaSettingsViewData);
                this.standardizedTests = SJX.fromArrayOfDeserializables(raw.standardizedtests, chlk.models.profile.StandardizedTestViewData);
                this.standardizedTestsStats = SJX.fromArrayOfDeserializables(raw.standardizedtestsstats, chlk.models.profile.StandardizedTestStatsViewData);
                this.dailyDisciplines = SJX.fromArrayOfDeserializables(raw.dailydisciplinestats, chlk.models.common.ChartDateItem);
                this.attendancesStats = SJX.fromArrayOfDeserializables(raw.attendancestats, chlk.models.common.ChartDateItem);
                this.studentDisciplineStats = SJX.fromArrayOfDeserializables(raw.disciplinestats, chlk.models.panorama.StudentDisciplineStatViewData);
                this.studentAbsenceStats = SJX.fromArrayOfDeserializables(raw.absences, chlk.models.panorama.StudentAbsenceStatViewData);

                this.attendancesDescending = false;
                this.disciplinesDescending = false;
                this.attendancesOrderBy = chlk.models.panorama.StudentAttendancesSortType.DATE;
                this.disciplinesOrderBy = chlk.models.panorama.StudentDisciplinesSortType.DATE;
            }
        ]);
});
