REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

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

            [[chlk.models.id.SchoolPersonId, String]],
            ria.async.Future, function changePassword(personId, password){
                return this.get('Person/ReChange/Password', Boolean, {
                    personId: personId.valueOf(),
                    newPassword: password
                });
            },

            [[chlk.models.id.SchoolPersonId, Number]],
            String, function getPictureURL(personId, size_){
                var url = window.azurePictureUrl + personId.valueOf();
                return size_ ? (url + '-' + size_ + 'x' + size_): url;
            }
        ])
});