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

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.StudentService*/
    CLASS(
        'StudentService', EXTENDS(chlk.services.BaseService), [

            chlk.models.student.StudentGradingInfo, 'currentStudentGradingSummary',

            ArrayOf(chlk.models.grading.ClassPersonGradingInfo), 'classPersonGradingInfo',

            [[chlk.models.id.ClassId, String, Boolean, Boolean, Number, Number]],
            ria.async.Future, function getStudents(classId_, filter_, myStudentsOnly_, byLastName_, start_, count_) {
                return this.getPaginatedList('Student/GetStudents.json', chlk.models.people.User, {
                    classId: classId_ && classId_.valueOf(),
                    filter: filter_,
                    myStudentsOnly: myStudentsOnly_,
                    byLastName: byLastName_,
                    start: start_,
                    count: count_
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

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Student/Info.json', chlk.models.student.StudentInfo, {
                    personId: personId.valueOf()
                });
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
                    console.info(model);
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