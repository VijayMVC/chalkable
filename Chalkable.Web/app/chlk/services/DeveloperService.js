REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.DeveloperId');
REQUIRE('chlk.models.developer.DeveloperInfo');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DeveloperService*/
    CLASS(
        'DeveloperService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.DeveloperId]],
            ria.async.Future, function getInfo(id) {
                return this.get('Developer/DeveloperInfo.json', chlk.models.developer.DeveloperInfo, {
                    developerId: id.valueOf()
                });
            },
            [[chlk.models.id.DeveloperId, String, String ,String, String]],
            ria.async.Future, function saveInfo(id, name, webSite, email) {
                return this.post('Developer/UpdateInfo.json', chlk.models.developer.DeveloperInfo, {
                    developerId: id.valueOf(),
                    name: name,
                    webSiteLink: webSite,
                    email: email
                });
            }
        ])
});