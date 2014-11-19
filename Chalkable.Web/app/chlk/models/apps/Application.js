REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppGradeLevelId');

REQUIRE('chlk.models.apps.AppPrice');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppPlatform');

REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.apps.AppScreenshots');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.BannedAppData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.Application */
    CLASS(
        UNSAFE,  'Application', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.AppId);
                this.message = SJX.fromValue(raw.message, String);
                this.internal = SJX.fromValue(raw.isinternal, Boolean);
                this.name = SJX.fromValue(raw.name, String);
                this.internal = SJX.fromValue(raw.internal, Boolean);
                this.url = SJX.fromValue(raw.url, String);
                this.videoDemoUrl = SJX.fromValue(raw.videodemourl, String);
                this.shortDescription = SJX.fromValue(raw.shortdescription, String);
                this.description = SJX.fromValue(raw.description, String);
                this.smallPictureId = SJX.fromValue(raw.smallpictureid, chlk.models.id.PictureId);
                this.bigPictureId = SJX.fromValue(raw.bigpictureid, chlk.models.id.PictureId);
                this.iconPicture = SJX.fromDeserializable(raw.iconpicture, chlk.models.apps.AppPicture);
                this.bannerPicture = SJX.fromDeserializable(raw.bannerPicture, chlk.models.apps.AppPicture);
                this.screenshotPictures = SJX.fromDeserializable(raw.screenshotpictures, chlk.models.apps.AppScreenShots);
                this.myAppsUrl = SJX.fromValue(raw.myappsurl, String);
                this.secretKey = SJX.fromValue(raw.secretkey, String);
                this.state = SJX.fromDeserializable(raw.state, chlk.models.apps.AppState);
                this.developerId = SJX.fromValue(raw.developerid, chlk.models.id.SchoolPersonId);
                this.developerInfo = SJX.fromDeserializable(raw.developer, chlk.models.developer.DeveloperInfo);
                this.liveAppId = SJX.fromValue(raw.liveappid, chlk.models.id.AppId);
                this.applicationPrice = SJX.fromDeserializable(raw.applicationprice, chlk.models.apps.AppPrice);
                this.screenshotIds = SJX.fromArrayOfValues(raw.picturesid, chlk.models.id.PictureId);
                this.appAccess = SJX.fromDeserializable(raw.applicationaccess, chlk.models.apps.AppAccess);
                this.permissions = SJX.fromArrayOfDeserializables(raw.permissions, chlk.models.apps.AppPermission);
                this.validRoles = SJX.fromArrayOfDeserializables(raw.canlaunchroles, chlk.models.people.Role);
                this.gradeLevels = SJX.fromArrayOfValues(raw.gradelevels, chlk.models.id.AppGradeLevelId);
                this.standardsCodes = SJX.fromArrayOfValues(raw.standardscodes, String);
                this.platforms = SJX.fromArrayOfDeserializables(raw.platforms, chlk.models.apps.AppPlatform);
                this.banInfo = SJX.fromDeserializable(raw.baninfo, chlk.models.apps.BannedAppData);
            },

            chlk.models.id.AppId, 'id',
            String, 'message',
            Boolean, 'internal',
            String, 'name',
            String, 'url',
            String, 'videoDemoUrl',
            String, 'shortDescription',
            String, 'description',
            chlk.models.id.PictureId, 'smallPictureId',
            chlk.models.id.PictureId, 'bigPictureId',
            chlk.models.apps.AppPicture, 'iconPicture',
            chlk.models.apps.AppPicture, 'bannerPicture',
            chlk.models.apps.AppScreenshots,  'screenshotPictures',
            String, 'myAppsUrl',
            String, 'secretKey',
            chlk.models.apps.AppState, 'state',
            chlk.models.id.SchoolPersonId, 'developerId',
            chlk.models.developer.DeveloperInfo, 'developerInfo',
            chlk.models.id.AppId, 'liveAppId',
            chlk.models.apps.AppPrice, 'applicationPrice',
            ArrayOf(chlk.models.id.PictureId), 'screenshotIds',
            chlk.models.apps.AppAccess, 'appAccess',
            ArrayOf(chlk.models.apps.AppPermission), 'permissions',
            ArrayOf(chlk.models.people.Role), 'validRoles',
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            ArrayOf(chlk.models.id.AppGradeLevelId), 'gradeLevels',
            ArrayOf(chlk.models.apps.AppPlatform), 'platforms',
            ArrayOf(String), 'standardsCodes',
            chlk.models.apps.BannedAppData, 'banInfo'
        ]);


});
