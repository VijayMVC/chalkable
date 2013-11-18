REQUIRE('chlk.services.BaseInfoService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.developer.DeveloperInfo');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DeveloperService*/
    CLASS(
        'DeveloperService', EXTENDS(chlk.services.BaseInfoService), [
            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(id) {
                return this.get('Developer/DeveloperInfo.json', chlk.models.developer.DeveloperInfo, {
                    developerId: id.valueOf()
                });
            },
            [[chlk.models.id.SchoolPersonId, String, String ,String]],
            ria.async.Future, function saveInfo(id, name, webSite, email) {
                return this
                    .post('Developer/UpdateInfo.json', chlk.models.developer.DeveloperInfo, {
                        developerId: id.valueOf(),
                        name: name,
                        webSiteLink: webSite,
                        email: email
                    })
                    .then(function(result){
                        this.userInfoChange.notify([result.getName()]);
                        return result;
                    }, this);
            }
        ])
});