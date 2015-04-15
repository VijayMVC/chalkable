REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppPersonReviewPostData*/
    CLASS(
        'AppPersonReviewPostData', [
            chlk.models.id.AppId, 'appId',
            Boolean, 'scroll',
            Number, 'start'
        ]);
});
