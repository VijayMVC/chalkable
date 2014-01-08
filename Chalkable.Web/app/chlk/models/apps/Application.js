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
    /** @class chlk.models.apps.Application */
    CLASS(
        'Application', [
            chlk.models.id.AppId, 'id',

            [ria.serialize.SerializeProperty('isinternal')],
            Boolean, 'internal',

            //[
                String, 'name',
                String, 'url',
                [ria.serialize.SerializeProperty('videodemourl')],
                String, 'videoDemoUrl',
                [ria.serialize.SerializeProperty('shortdescription')],
                String, 'shortDescription',
                String, 'description',
                [ria.serialize.SerializeProperty('smallpictureid')],
                chlk.models.id.PictureId, 'smallPictureId',
                [ria.serialize.SerializeProperty('bigpictureid')],
                chlk.models.id.PictureId, 'bigPictureId',
            //]

            chlk.models.apps.AppPicture, 'iconPicture',
            chlk.models.apps.AppPicture, 'bannerPicture',
            chlk.models.apps.AppScreenshots,  'screenshotPictures',

            [ria.serialize.SerializeProperty('myappsurl')],
            String, 'myAppsUrl',
            [ria.serialize.SerializeProperty('secretkey')],
            String, 'secretKey',
            chlk.models.apps.AppState, 'state',

            [ria.serialize.SerializeProperty('developerid')],
            chlk.models.id.SchoolPersonId, 'developerId',

            [ria.serialize.SerializeProperty('developer')],
            chlk.models.developer.DeveloperInfo, 'developerInfo',

            [ria.serialize.SerializeProperty('liveappid')],
            chlk.models.id.AppId, 'liveAppId',
            [ria.serialize.SerializeProperty('applicationprice')],
            chlk.models.apps.AppPrice, 'applicationPrice',
            [ria.serialize.SerializeProperty('picturesid')],
            ArrayOf(chlk.models.id.PictureId), 'screenshotIds',
            [ria.serialize.SerializeProperty('applicationaccess')],
            chlk.models.apps.AppAccess, 'appAccess',
            ArrayOf(chlk.models.apps.AppPermission), 'permissions',
            [ria.serialize.SerializeProperty('canlaunchroles')],
            ArrayOf(chlk.models.people.Role), 'validRoles',

            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            [ria.serialize.SerializeProperty('gradelevels')],
            ArrayOf(chlk.models.id.AppGradeLevelId), 'gradeLevels',

            ArrayOf(chlk.models.apps.AppPlatform), 'platforms',

            chlk.models.apps.BannedAppData, 'banInfo'
        ]);


});
