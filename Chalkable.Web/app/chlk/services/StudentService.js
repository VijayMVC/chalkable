REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.StudentService*/
    CLASS(
        'StudentService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.ClassId, String, Boolean, Number, Number, Number]],
            ria.async.Future, function getStudents(classId_, filter_, myStudentsOnly_, sortType_, start_, count_) {
                return this.get('Student/GetStudents.json', chlk.models.people.User, {
                    classId: classId_ && classId_.valueOf(),
                    filter: filter_,
                    myStudentsOnly: myStudentsOnly_,
                    sortType: sortType_,
                    start: start_,
                    count: count_
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Student/Info.json', chlk.models.people.User, {
                    personId: personId.valueOf()
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
            }
        ])
});