REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppGradeLevelId');

REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppPlatform');

REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.apps.AppScreenShots');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.academicBenchmark.Standard');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    function valueOf(x) { return x != null && x.valueOf ? x.valueOf() : x}

    /** @class chlk.models.apps.Application */
    CLASS(
        UNSAFE,  'Application', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.AppId);
                this.message = SJX.fromValue(raw.message, String);
                this.internal = SJX.fromValue(raw.isinternal, Boolean);
                this.name = SJX.fromValue(raw.name, String);
                this.url = SJX.fromValue(raw.url, String);
                this.videoDemoUrl = SJX.fromValue(raw.videodemourl, String);
                this.shortDescription = SJX.fromValue(raw.shortdescription, String);
                this.description = SJX.fromValue(raw.description, String);
                this.smallPictureId = SJX.fromValue(raw.smallpictureid, chlk.models.id.PictureId);
                this.bigPictureId = SJX.fromValue(raw.bigpictureid, chlk.models.id.PictureId);
                this.externalAttachPictureId = SJX.fromValue(raw.externalattachpictureid, chlk.models.id.PictureId);
                this.iconPicture = SJX.fromDeserializable(raw.iconpicture, chlk.models.apps.AppPicture);
                this.bannerPicture = SJX.fromDeserializable(raw.bannerPicture, chlk.models.apps.AppPicture);
                this.externalAttachPicture = SJX.fromDeserializable(raw.externalattachpicture, chlk.models.apps.AppPicture);
                this.screenshotPictures = SJX.fromDeserializable(raw.screenshotpictures, chlk.models.apps.AppScreenShots);
                this.myAppsUrl = SJX.fromValue(raw.myappsurl, String);
                this.secretKey = SJX.fromValue(raw.secretkey, String);
                this.encodedSecretKey = SJX.fromValue(raw.encodedsecretkey, String);
                this.state = SJX.fromDeserializable(raw.state, chlk.models.apps.AppState);
                this.developerId = SJX.fromValue(raw.developerid, chlk.models.id.SchoolPersonId);
                this.developerInfo = SJX.fromDeserializable(raw.developer, chlk.models.developer.DeveloperInfo);
                this.liveAppId = SJX.fromValue(raw.liveappid, chlk.models.id.AppId);
                this.screenshotIds = SJX.fromArrayOfValues(raw.picturesid, chlk.models.id.PictureId);
                this.appAccess = SJX.fromDeserializable(raw.applicationaccess, chlk.models.apps.AppAccess);
                this.permissions = SJX.fromArrayOfDeserializables(raw.permissions, chlk.models.apps.AppPermission);
                this.validRoles = SJX.fromArrayOfDeserializables(raw.canlaunchroles, chlk.models.people.Role);
                this.gradeLevels = SJX.fromArrayOfValues(raw.gradelevels, chlk.models.id.AppGradeLevelId);
                this.standardsIds = SJX.fromArrayOfValues(raw.standardsids, String);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.academicBenchmark.Standard);
                this.platforms = SJX.fromArrayOfDeserializables(raw.platforms, chlk.models.apps.AppPlatform);
                this.banned = SJX.fromValue(raw.ban, Boolean);
                this.categories = SJX.fromArrayOfDeserializables(raw.categories, chlk.models.apps.AppCategory);
                this.internalScore = SJX.fromValue(raw.internalscore, Number);
                this.internalDescription = SJX.fromValue(raw.internaldescription, String);
                this.advancedApp = SJX.fromValue(raw.isadvanced, Boolean);


                if(raw.liveapplication)
                    this.liveApplication = SJX.fromDeserializable(raw.liveapplication, SELF);

                this.bannedForCurrentSchool = SJX.fromValue(raw.isbannedforcurrentschool, Boolean);
                this.bannedForDistrict = SJX.fromValue(raw.isbannedfordistrict, Boolean);
                this.partiallyBanned = SJX.fromValue(raw.ispartiallybanned, Boolean);

                this.accessToken = SJX.fromValue(raw.accesstoken, String);
            },

            chlk.models.id.AppId, 'id',
            String, 'message',
            Boolean, 'internal',
            Boolean, 'advancedApp',
            String, 'name',
            String, 'url',
            String, 'videoDemoUrl',
            String, 'shortDescription',
            String, 'description',
            chlk.models.id.PictureId, 'smallPictureId',
            chlk.models.id.PictureId, 'bigPictureId',
            chlk.models.id.PictureId, 'externalAttachPictureId',

            chlk.models.apps.AppPicture, 'iconPicture',
            chlk.models.apps.AppPicture, 'bannerPicture',
            chlk.models.apps.AppPicture, 'externalAttachPicture',

            chlk.models.apps.AppScreenShots,  'screenshotPictures',
            String, 'myAppsUrl',
            String, 'secretKey',
            String, 'encodedSecretKey',
            chlk.models.apps.AppState, 'state',
            chlk.models.id.SchoolPersonId, 'developerId',
            chlk.models.developer.DeveloperInfo, 'developerInfo',
            chlk.models.id.AppId, 'liveAppId',
            ArrayOf(chlk.models.id.PictureId), 'screenshotIds',
            chlk.models.apps.AppAccess, 'appAccess',
            ArrayOf(chlk.models.apps.AppPermission), 'permissions',
            ArrayOf(chlk.models.people.Role), 'validRoles',
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            ArrayOf(chlk.models.id.AppGradeLevelId), 'gradeLevels',
            ArrayOf(chlk.models.apps.AppPlatform), 'platforms',
            ArrayOf(chlk.models.id.ABStandardId), 'standardsIds',
            ArrayOf(chlk.models.academicBenchmark.Standard), 'standards',
            Boolean, 'banned',

            Boolean, 'bannedForCurrentSchool',
            Boolean, 'bannedForDistrict',
            Boolean, 'partiallyBanned',

            Number, 'internalScore',
            String, 'internalDescription',
            String, 'accessToken',


            SELF, 'liveApplication',

            READONLY, Boolean, 'live',
            Boolean, function isLive(){
                return this.getState() && this.getState().getStateId() == chlk.models.apps.AppStateEnum.LIVE;
            },

            String, function getExternalAttachPictureUrl(){
                var dims = chlk.models.apps.AppPicture.EXTERNAL_ATTACH_ICON_DIMS();
                return window.azurePictureUrl + valueOf(this.getExternalAttachPictureId()) + '-' + dims.width + 'x' + dims.height;
            },

            String, function getIconPictureUrl(){
                var dims = chlk.models.apps.AppPicture.ICON_DIMS();
                return window.azurePictureUrl + valueOf(this.getSmallPictureId()) + '-' + dims.width + 'x' + dims.height;
            },

            String, function getBannerPictureUrl(){
                var dims = chlk.models.apps.AppPicture.BANNER_DIMS();
                var id = valueOf(this.getBigPictureId() || this.getSmallPictureId());
                return window.azurePictureUrl + id + '-' + dims.width + 'x' + dims.height;
            },

            READONLY, String, 'statusText',
            String, function getStatusText(){
                var state = this.getState();
                if(this.getLiveAppId()){
                    switch(state.getStateId()){
                        case chlk.models.apps.AppStateEnum.SUBMITTED_FOR_APPROVAL.valueOf():
                            return 'Live - Update awaiting approval';
                        case chlk.models.apps.AppStateEnum.APPROVED.valueOf():
                            return 'Live - Update approved';
                        case chlk.models.apps.AppStateEnum.REJECTED.valueOf():
                            return 'Live - Update rejected';
                        default :
                            return state.toString(); //'Your app is live in the Chalkable App Store';
                    }
                }
                return state.toString()
            }
        ]);


});
