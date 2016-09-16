REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.schoolImport.School');
REQUIRE('chlk.models.school.SchoolDetails');
REQUIRE('chlk.models.school.Timezone');
REQUIRE('chlk.models.school.SchoolSisInfo');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.Success');
REQUIRE('chlk.models.recipients.Program');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SchoolService */
    CLASS(
        'SchoolService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.DistrictId, Number, Number]],
            ria.async.Future, function getSchools(districtId, start_, count_) {
                return this.getPaginatedList('School/List.json', chlk.models.school.School, {
                    start: start_,
                    districtId: districtId.valueOf(),
                    count: count_
                });
            },


            [[chlk.models.id.DistrictId]],
            ria.async.Future, function getSchoolsForImport(districtId) {
                return this.getPaginatedList('School/GetSchoolsForImport.json', chlk.models.schoolImport.School, {
                    districtId: districtId.valueOf()
                });
            },

            [[chlk.models.id.DistrictId, Number, Number]],
            ria.async.Future, function runSchoolImport(districtId, sisSchoolId, sisSchoolYearId) {
                return this.get('School/RunSchoolImport.json', Boolean, {
                    districtId: districtId.valueOf(),
                    sisSchoolId: sisSchoolId,
                    sisSchoolYearId: sisSchoolYearId
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('School/Summary.json', chlk.models.school.SchoolDetails, {
                    schoolId: schoolId.valueOf()
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function getSisInfo(schoolId) {
                return this.get('chalkable2/data/schoolSisInfo.json', chlk.models.school.SchoolSisInfo, {
                    schoolId: schoolId.valueOf()
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function updateMessagingSettings(schoolId_, studentMessaging_, studentToClassOnly_,
                                                               teacherToStudentMessaging_, teacherToClassOnly_) {
                return this.get('School/UpdateMessagingSettings.json', Boolean, {
                    schoolId: schoolId_ && schoolId_.valueOf(),
                    studentMessaging: studentMessaging_,
                    studentToClassOnly: studentToClassOnly_ || false,
                    teacherToStudentMessaging: teacherToStudentMessaging_,
                    teacherToClassOnly: teacherToClassOnly_ || false
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('School/People.json', chlk.models.school.SchoolPeopleSummary, {
                    schoolId: schoolId.valueOf()
                });
            },

            [[chlk.models.id.SchoolId, String, String, Number, Number, Boolean]],
            ria.async.Future, function getUsers(schoolId, rolesId, gradeLevelsIds, start, count, byLastName){
                return this.getPaginatedList('School/GetPersons.json', chlk.models.people.User, {
                    schoolId: schoolId.valueOf(),
                    start: start,
                    count: count,
                    rolesId: rolesId,
                    gradeLevelId: gradeLevelsIds,
                    byLastName: byLastName
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function del(schoolId) {
                return this.post('School/delete.json', chlk.models.Success, {
                    schoolId: schoolId.valueOf()
                });
            },

            ria.async.Future, function getTimezones() {
                return this.getPaginatedList('School/ListTimezones.json', chlk.models.school.Timezone, {});
            },

            ria.async.Future, function getSchoolPrograms() {
                return this.get('School/SchoolPrograms.json', chlk.models.recipients.Program, {});
            },

            [[chlk.models.id.DistrictId, chlk.models.id.SchoolId, chlk.models.common.ChlkDate]],
            ria.async.Future, function updateStudyCenter(districtId_, schoolId_, enabledTill_){
                return this.post('School/UpdateStudyCenterEnabled.json', Boolean,{
                    districtId: districtId_ && districtId_.valueOf(),
                    schoolId: schoolId_ && schoolId_.valueOf(),
                    enabledTill: enabledTill_ && enabledTill_.toStandardFormat()
                });
            },

            [[chlk.models.id.DistrictId, chlk.models.id.SchoolId, Boolean]],
            ria.async.Future, function updateAssessmentEnabled(districtId, schoolId_, enabled){
                return this.post('School/UpdateAssessmentEnabled.json', Boolean,{
                    districtId: districtId && districtId.valueOf(),
                    schoolId: schoolId_ && schoolId_.valueOf(),
                    enabled: enabled
                });
            }
        ])
});