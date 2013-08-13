REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppGradeLevelId');

REQUIRE('chlk.models.apps.AppPrice');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.common.NameId');


NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.Application*/
    CLASS(
        'Application', [
            chlk.models.id.AppId, 'id',

            [ria.serialize.SerializeProperty('isinternal')],
            Boolean, 'isInternal',

            //[
                String, 'name',
                String, 'url',
                [ria.serialize.SerializeProperty('videodemourl')],
                String, 'videoModeUrl',
                [ria.serialize.SerializeProperty('shortdescription')],
                String, 'shortDescription',
                String, 'description',
                [ria.serialize.SerializeProperty('smallpictureid')],
                chlk.models.id.PictureId, 'smallPictureId',
                [ria.serialize.SerializeProperty('bigpictureid')],
                chlk.models.id.PictureId, 'bigPictureId',
            //]



            [ria.serialize.SerializeProperty('myappsurl')],
            String, 'myAppsUrl',
            [ria.serialize.SerializeProperty('secretkey')],
            String, 'secretKey',
            chlk.models.apps.AppState, 'state',
            [ria.serialize.SerializeProperty('developerid')],
            chlk.models.id.SchoolPersonId, 'developerId',
            [ria.serialize.SerializeProperty('liveappid')],
            chlk.models.id.AppId, 'liveAppId',
            [ria.serialize.SerializeProperty('isinternal')],
            Boolean, 'isInternal',
            [ria.serialize.SerializeProperty('applicationprice')],
            chlk.models.apps.AppPrice, 'applicationPrice',
            [ria.serialize.SerializeProperty('picturesid')],
            ArrayOf(chlk.models.id.PictureId), 'pictureIds',
            [ria.serialize.SerializeProperty('applicationaccess')],
            chlk.models.apps.AppAccess, 'appAccess',
            ArrayOf(chlk.models.apps.AppPermission), 'permissions',
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            [ria.serialize.SerializeProperty('gradelevels')],
            ArrayOf(chlk.models.id.AppGradeLevelId), 'gradeLevels'
        ]);


});
