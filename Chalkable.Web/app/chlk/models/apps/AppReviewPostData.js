REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppReviewPostData*/
    CLASS(
        'AppReviewPostData', [
            [ria.serialize.SerializeProperty('reviewText')],
            String, 'review',


            [ria.serialize.SerializeProperty('newRating')],
            Number, 'rating',

            chlk.models.id.AppId, 'appId'
        ]);
});
