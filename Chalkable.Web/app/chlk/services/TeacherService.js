REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.TeacherService*/
    CLASS(
        'TeacherService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Teacher/Info.json', chlk.models.people.User, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, String, String, String, String, String, String, String, chlk.models.common.ChlkDate]],
            ria.async.Future, function updateInfo(personId, addresses, email, firstName, lastName, gender, phones, salutation, birthDate) {
                return this.post('Teacher/UpdateInfo.json', chlk.models.people.User, {
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

            [[String, chlk.models.id.ClassId, Boolean, Number]],
            ria.async.Future, function getTeacher(filter, classId, byLastName, start){
                return this.PaginatedList('Teacher/GetTeacher.json', chlk.models.people.User, {
                    filter: filter,
                    classId: classId.valueOf(),
                    byLastName: byLastName,
                    start: start
                });
            }
        ])
});