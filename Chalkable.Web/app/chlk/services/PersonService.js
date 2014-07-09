REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.people.User');


REQUIRE('chlk.models.id.SchoolPersonId');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.PersonService */
    CLASS(
        'PersonService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId, Object]],
            ria.async.Future, function uploadPicture(personId, files) {
                return this.uploadFiles('Person/UploadPicture', files, Boolean, {
                    personId: personId.valueOf()
                });
            },

            [[String]],
            ria.async.Future, function changeEmail(email) {
                return this.get('User/ActivateUser', Boolean, {
                    newUserEmail: email
                });
            },

            [[chlk.models.id.SchoolPersonId, String]],
            ria.async.Future, function changePassword(personId, password){
                return this.get('Person/ReChangePassword', Boolean, {
                    id: personId.valueOf(),
                    newPassword: password
                });
            },

            [[chlk.models.id.SchoolPersonId, String, String]],
            ria.async.Future, function updateInfo(personId, email, phones) {
                return this.post('Person/UpdateInfo.json', chlk.models.people.User, {
                    personId: personId.valueOf(),
                    email: email,
                    phones: phones && JSON.parse(phones)
                });
            },

            [[chlk.models.id.SchoolPersonId, Number]],
            String, function getPictureURL(personId, size_){
                var url = window.azurePictureUrl + window.school.districtid + '_' + personId.valueOf();
                return size_ ? (url + '-' + size_ + 'x' + size_): url;
            },

            [[String]],
            ria.async.Future, function getPersons(filter_) {
                return this.getPaginatedList('Person/GetPersons.json', chlk.models.people.User, {
                    filter: filter_,
                    start: 0,
                    count: 10000
                })
                .then(function(model){return model.getItems();});
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getSchedule(personId) {
                return this.get('Person/Schedule.json', chlk.models.people.Schedule, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getAppsInfo(personId){
                return this.get('Person/Apps.json', chlk.models.people.PersonApps,{
                    personId: personId && personId.valueOf()
                });
            }
        ])
});