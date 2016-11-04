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

    /** @class chlk.models.apps.ApplicationForPanoramaViewData */
    CLASS(
        UNSAFE,  'ApplicationForPanoramaViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.AppId);
                this.name = SJX.fromValue(raw.name, String);
                this.url = SJX.fromValue(raw.url, String);
                this.viewUrl = SJX.fromValue(raw.viewurl, String);
                this.accessToken = SJX.fromValue(raw.accesstoken, String);
            },

            chlk.models.id.AppId, 'id',
            String, 'name',
            String, 'url',
            String, 'viewUrl',
            String, 'accessToken'
        ]);


});
