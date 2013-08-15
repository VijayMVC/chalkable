REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('ria.async.Observable');

REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppPermissionId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DevApplicationListChangeEvent */
    DELEGATE(
        [[ArrayOf(chlk.models.apps.Application)]],
        VOID, function DevApplicationListChangeEvent(list) {});

    /** @class chlk.services.ApplicationService */
    CLASS(
        'ApplicationService', EXTENDS(chlk.services.BaseService), [

            READONLY, ria.async.IObservable, 'devApplicationListChange',

            function $() {
                BASE();
                this.devApplicationListChange = new ria.async.Observable(chlk.services.DevApplicationListChangeEvent);
            },

            [[Number]],
            ria.async.Future, function getApps(pageIndex_) {
                return this.getPaginatedList('Application/List.json', chlk.models.apps.Application, {
                        start: pageIndex_
                    })
            },

            [[chlk.models.apps.Application]],
            function switchApp(app){
                this.getContext().getSession().set('currentApp', app);
            },

            function getCurrentApp(){
                return this.getContext().getSession().get('currentApp') || {};
            },

            [[Boolean]],
            ria.async.Future, function getDevApps(refresh_) {
                var apps = this.getContext().getSession().get('dev-apps') || [];

                return apps.length == 0 || refresh_
                    ? this.getPaginatedList('Application/List.json', chlk.models.apps.Application, {})
                          .then(function(data){
                                this.getContext().getSession().set('dev-apps', data.getItems());
                                return data.getItems();
                    }, this)
                    : new ria.async.DeferredData(apps);
            },


            Array, function arrayToIds(obj){
                return obj ? obj.map(function(item){ return item.valueOf();}) : [];
            },

            String, function arrayToCsv(obj){
                return obj ? obj.map(function(item){ return item.valueOf();}).join(',') : "";
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function getInfo(appId_) {
                return appId_
                    ? this.get('Application/GetInfo.json', chlk.models.apps.Application, {applicationId: appId_.valueOf()})
                    : ria.async.DeferredData(this.getCurrentApp());

            },
            [[chlk.models.id.SchoolPersonId, String]],
            ria.async.Future, function createApp(devId, name) {
                return this
                    .post('Application/Create.json', chlk.models.apps.Application, {
                        developerId: devId.valueOf(),
                        name: name
                    })
                    .then(function (data) {
                        return this.getDevApps(true)
                        .then(function(items){
                            this.devApplicationListChange.notify([items]);
                            this.switchApp(data);
                            return data;
                        }, this)
                    }, this);
            },
            [[chlk.models.id.AppId]],
            ria.async.Future, function deleteApp(appId) {
                return this
                    .post('Application/Delete.json', Boolean, {applicationId: appId.valueOf()})
                    .then(function (data) {
                        return this.getDevApps(true)
                            .then(function(items){
                                this.devApplicationListChange.notify([items]);
                                this.switchApp(items.length > 0 ? items[items.length - 1] : new chlk.models.apps.Application());
                                return data;
                            }, this)
                    }, this);
            },
            [[
                chlk.models.id.AppId,
                chlk.models.apps.ShortAppInfo,
                ArrayOf(chlk.models.id.AppPermissionId),
                chlk.models.apps.AppPrice,
                chlk.models.id.SchoolPersonId,
                chlk.models.apps.AppAccess,
                ArrayOf(chlk.models.id.AppCategoryId),
                ArrayOf(chlk.models.id.PictureId),
                ArrayOf(chlk.models.id.GradeLevelId),
                Boolean
            ]],
            ria.async.Future, function updateApp(
                appId, shortAppInfo, permissionIds, appPricesInfo, devId, appAccess, categories, pictures_, gradeLevels, forSubmit){
                return this.post('Application/Update.json', chlk.models.apps.Application,  {
                    applicationId: appId.valueOf(),
                    shortApplicationInfo: shortAppInfo.getPostData(),
                    permissions: this.arrayToIds(permissionIds),
                    applicationPrices: appPricesInfo.getPostData(),
                    developerId: devId.valueOf(),
                    applicationAccessInfo: appAccess.getPostData(),
                    categories: this.arrayToIds(categories),
                    //add pictures
                    gradeLevels: this.arrayToIds(gradeLevels),
                    forSubmit: forSubmit
                })
                //here only if name was changed
            },


            ArrayOf(chlk.models.apps.AppPermission), function getAppPermissions(){
                return  [
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(0), "User"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(1), "Message"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(2), "Grade"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(3), "Attendance"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(4), "Announcement"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(5), "Class"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(6), "Schedule"),
                    new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(7), "Discipline")
                ];
            }



        ])
});