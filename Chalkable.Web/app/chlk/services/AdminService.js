REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolId');

REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminService */
    CLASS(
        'AdminService', EXTENDS(chlk.services.BaseService), [
            [[Number, String, Boolean, Number, String]],
            ria.async.Future, function getUsers(roleId, gradeLevelIds, byLastName, start, filter) {
                return this.getPaginatedList('Admin/GetPersons.json', chlk.models.people.User, {
                    start: start,
                    roleId: roleId || null,
                    gradeLevelIds: gradeLevelIds,
                    byLastName: byLastName,
                    filter: filter
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Admin/Info.json', chlk.models.people.User, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, String, String, String, String, String, String, String, chlk.models.common.ChlkDate]],
            ria.async.Future, function updateInfo(personId, addresses, email, firstName, lastName, gender, phones, salutation, birthDate) {
                return this.post('Admin/UpdateInfo.json', chlk.models.people.User, {
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