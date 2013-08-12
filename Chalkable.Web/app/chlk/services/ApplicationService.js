REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ApplicationService */
    CLASS(
        'ApplicationService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getApps(pageIndex_) {
                return this.getPaginatedList('Application/List.json', chlk.models.apps.Application, {
                    start: pageIndex_
                });
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function getInfo(appId) {
                return this.get('Application/GetInfo.json', chlk.models.apps.Application, {
                    applicationId: appId.valueOf()
                });
            },
            [[chlk.models.id.SchoolPersonId, String]],
            ria.async.Future, function createApp(devId, name) {
                return this.post('Application/Create.json', chlk.models.apps.Application, {
                    developerId: devId.valueOf(),
                    name: name
                });
            },
            [[chlk.models.id.AppId]],
            ria.async.Future, function deleteApp(appId) {
                return this.post('Application/Delete.json', Boolean, {
                    applicationId: appId.valueOf()
                });
            }


        ])
});