REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.student.StudentInfo');
REQUIRE('chlk.models.student.StudentGradingInfo');
REQUIRE('chlk.models.people.Schedule');
REQUIRE('chlk.models.student.StudentExplorer');
REQUIRE('chlk.models.people.PersonApps');
REQUIRE('chlk.models.panorama.StudentPanoramaViewData');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.StudentService*/
    CLASS(
        'StudentService', EXTENDS(chlk.services.BaseService), [

            chlk.models.student.StudentGradingInfo, 'currentStudentGradingSummary',

            ArrayOf(chlk.models.grading.ClassPersonGradingInfo), 'classPersonGradingInfo',

            [[chlk.models.id.ClassId, String, Boolean, Boolean, Number, Number, Boolean,
                chlk.models.id.SchoolId, chlk.models.id.GradeLevelId, chlk.models.id.ProgramId]],
            ria.async.Future, function getStudents(classId_, filter_, myStudentsOnly_, byLastName_, start_, count_, enrolledOnly_,
                                                   schoolId_, gradeLevel_, programId_) {
                return this.getPaginatedList('Student/GetStudents.json', chlk.models.people.User, {
                    classId: classId_ && classId_.valueOf(),
                    filter: filter_,
                    myStudentsOnly: myStudentsOnly_,
                    byLastName: byLastName_,
                    start: start_,
                    count: count_,
                    enrolledOnly: enrolledOnly_,
                    schoolId: schoolId_ && schoolId_.valueOf(),
                    gradeLevel: gradeLevel_ && gradeLevel_.valueOf(),
                    programId: programId_ && programId_.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getClassStudents(classId, gradingPeriodId){
                //return this.get('Student/GetClassStudents.json', ArrayOf(chlk.models.people.User),{
                //    classId: classId,
                //    gradingPeriodId: gradingPeriodId
                //});
                return this.getStudents(classId, null, null, true, 0, 10000).then(function(data){
                    return data.getItems();
                });
            },

            [[String]],
            ria.async.Future, function getStudentsByFilter(filter_) {
                return this.getStudents(null, filter_, true, true)
                           .then(function(model){return model.getItems();});
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.HealthFormId]],
            ria.async.Future, function verifyStudentHealthForm(studentId, healthFormId) {
                return this.get('Student/VerifyStudentHealthForm.json', ArrayOf(chlk.models.student.StudentHealthFormViewData), {
                    studentId: studentId.valueOf(),
                    healthFormId: healthFormId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.HealthFormId]],
            String, function getHealthFormDocumentUri(studentId, healthFormId) {
                //return "https://local.chalkable.com/Content/sample-3pp.pdf";
                return this.getUrl('Student/DownloadHealthFormDocument', {
                    studentId: studentId.valueOf(),
                    healthFormId: healthFormId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Student/Info.json', chlk.models.student.StudentInfo, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfoForPanorama(personId) {
                return this.get('Student/Info.json', chlk.models.student.StudentPanoramaViewData, {
                    personId: personId.valueOf()
                });
            },

            ria.async.Future, function getPanorama(studentId, data_) {
                return this.post('Student/Panorama.json', chlk.models.panorama.StudentPanoramaViewData, {
                    studentId: studentId.valueOf(),
                    standardizedTestFilters: data_ && data_.standardizedTestFilters,
                    acadYears: data_ && data_.acadYears
                });
            },

            ria.async.Future, function savePanoramaSettings(studentId, data) {
                return this.post('Student/SavePanoramaSettings.json', Boolean, {
                    studentId : studentId.valueOf(),
                    standardizedTestFilters: data.standardizedTestFilters,
                    acadYears: data.acadYears
                });
            },

            ria.async.Future, function restorePanorama() {
                return this.get('Student/RestorePanoramaSettings.json', Object, {});
            },

            [[chlk.models.panorama.StudentAttendancesSortType, Boolean]],
            ria.async.Future, function sortPanoramaAttendances(orderBy_, descending_){
                orderBy_ = orderBy_ || chlk.models.panorama.StudentAttendancesSortType.DATE;
                descending_ = descending_ || false;
                var panorama = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PANORAMA, null).getPanoramaInfo();

                if(!panorama)
                    throw 'No panorama saved';

                var items = panorama.getStudentAbsenceStats() || [], propRef, propName,
                    ref = ria.reflection.ReflectionClass(chlk.models.panorama.StudentAbsenceStatViewData);

                switch (orderBy_){
                    case chlk.models.panorama.StudentAttendancesSortType.DATE: propName = 'date'; break;
                    case chlk.models.panorama.StudentAttendancesSortType.REASON: propName = 'absenceReasonName'; break;
                    case chlk.models.panorama.StudentAttendancesSortType.LEVEL: propName = 'absenceLevel'; break;
                    case chlk.models.panorama.StudentAttendancesSortType.CATEGORY: propName = 'absenceCategory'; break;
                    case chlk.models.panorama.StudentAttendancesSortType.PERIODS: propName = 'periods'; break;
                    case chlk.models.panorama.StudentAttendancesSortType.NOTE: propName = 'note'; break;
                }

                propRef = ref.getPropertyReflector(propName);

                items = items.sort(function(s1, s2){
                    var a, b;

                    a = propRef.invokeGetterOn(s1);
                    b = propRef.invokeGetterOn(s2);

                    if(a && a.getDate && b && b.getDate){
                        a = a.getDate();
                        b = b.getDate();
                    }

                    if(a && Array.isArray(a) && b && Array.isArray(b)){
                        a = a.length;
                        b = b.length;
                    }

                    if (a > b)
                        return descending_ ? -1 : 1;
                    if (a < b)
                        return descending_ ? 1 : -1;
                    return 0
                });

                panorama.setAttendancesOrderBy(orderBy_);
                panorama.setAttendancesDescending(descending_);

                return ria.async.DeferredData(panorama);
            },

            [[chlk.models.panorama.StudentDisciplinesSortType, Boolean]],
            ria.async.Future, function sortPanoramaDisciplines(orderBy_, descending_){
                orderBy_ = orderBy_ || chlk.models.panorama.StudentDisciplinesSortType.DATE;
                descending_ = descending_ || false;
                var panorama = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PANORAMA, null).getPanoramaInfo();

                if(!panorama)
                    throw 'No panorama saved';

                var items = panorama.getStudentDisciplineStats() || [], propRef, propName,
                    ref = ria.reflection.ReflectionClass(chlk.models.panorama.StudentDisciplineStatViewData);

                switch (orderBy_){
                    case chlk.models.panorama.StudentDisciplinesSortType.DATE: propName = 'occurrenceDate'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.INFRACTION: propName = 'infractionName'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.CODE: propName = 'infractionStateCode'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.DEMERITS: propName = 'demeritsEarned'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.PRIMARY: propName = 'primary'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.DISPOSITION_DATE: propName = 'dispositionStartDate'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.DISPOSITION: propName = 'dispositionName'; break;
                    case chlk.models.panorama.StudentDisciplinesSortType.NOTE: propName = 'dispositionNote'; break;
                }

                propRef = ref.getPropertyReflector(propName);

                items = items.sort(function(s1, s2){
                    var a, b;

                    a = propRef.invokeGetterOn(s1);
                    b = propRef.invokeGetterOn(s2);

                    if(a && a.getDate && b && b.getDate){
                        a = a.getDate();
                        b = b.getDate();
                    }

                    if (a > b)
                        return descending_ ? -1 : 1;
                    if (a < b)
                        return descending_ ? 1 : -1;
                    return 0
                });

                panorama.setDisciplinesOrderBy(orderBy_);
                panorama.setDisciplinesDescending(descending_);

                return ria.async.DeferredData(panorama);
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getSummary(personId) {
                return this.get('Student/Summary.json', chlk.models.student.StudentSummary, {
                    schoolPersonId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, String, String, String, String, String, String, String, chlk.models.common.ChlkDate]],
            ria.async.Future, function updateInfo(personId, addresses, email, firstName, lastName, gender, phones, salutation, birthDate) {
                return this.post('Student/UpdateInfo.json', chlk.models.people.User, {
                    personId: personId.valueOf(),
                    addresses: addresses && JSON.parse(addresses),
                    email: email,
                    firstName: firstName,
                    lastName: lastName,
                    gender: gender,
                    phones: phones && JSON.parse(phones),
                    salutation: salutation,
                    birthdayDate: birthDate && JSON.stringify(birthDate.getDate()).slice(1,-1)
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getGradingInfo(studentId, gradingPeriodId_){
                return this.get('Student/GradingSummary.json', chlk.models.student.StudentGradingInfo, {
                    studentId: studentId && studentId.valueOf(),
                    gradingPeriodId: gradingPeriodId_ && gradingPeriodId_.valueOf()
                }).then(function(model){
                    this.setCurrentStudentGradingSummary(model);
                    return model;
                }, this);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getGradingDetailsForPeriod(studentId, gradingPeriodId){
                return this.get('Student/GradingDetails.json', Object, {
                    studentId: studentId && studentId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf()
                }).then(function(items){
                    items = ria.serialize.SJX.fromArrayOfDeserializables(items.classavgs, chlk.models.grading.ClassPersonGradingInfo);
                    this.setClassPersonGradingInfo(items);
                    var model = this.getCurrentStudentGradingSummary().getGradesByGradingPeriod().filter(function(item){return item.getGradingPeriod().getId() == gradingPeriodId })[0];
                    model.getStudentGradings().forEach(function(item, i){
                        var current = items.filter(function(gradingItem){
                            return item.getClazz().getId() == gradingItem.getClassId()
                        })[0];
                        if(current){
                            var info = current.getGradingByAnnouncementTypes();
                            info && item.setGradingByAnnouncementTypes(info);
                        }
                    });
                    return model;
                }, this);
            },

            [[Number, chlk.models.id.ClassId]],
            chlk.models.grading.ClassPersonGradingItem, function getGradingActivitiesForStudent(announcementTypeId, classId){
                var item = this.getClassPersonGradingInfo().filter(function(classInfo){
                    return classInfo.getClassId() == classId
                })[0].getGradingByAnnouncementTypes().filter(function(typeInfo){
                        return typeInfo.getAnnouncementType().getId() == announcementTypeId
                    })[0];
                return item;
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getSchedule(personId) {
                return this.get('Student/Schedule.json', chlk.models.people.Schedule, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getExplorer(personId){
                return this.get('Student/Explorer.json', chlk.models.student.StudentExplorer, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getStudentAttendanceSummary(studentId, gradingPeriodId) {
                return this.get('Student/AttendanceSummary.json', chlk.models.attendance.StudentAttendanceSummary, {
                    studentId: studentId && studentId.valueOf(),
                    gradingPeriodId: gradingPeriodId && gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getStudentDisciplineSummary(personId, gradingPeriodId){
                return this.get('Student/DisciplineSummary.json', chlk.models.discipline.StudentDisciplineSummary ,{
                    studentId: personId && personId.valueOf(),
                    gradingPeriodId: gradingPeriodId && gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, Number, Number]],
            ria.async.Future, function getAppsInfo(studentId, start_, count_){
                return this.get('Student/Apps.json', chlk.models.people.PersonApps, {
                    studentId: studentId.valueOf(),
                    start: start_ || 0,
                    count: count_
                });
            }
        ])
});