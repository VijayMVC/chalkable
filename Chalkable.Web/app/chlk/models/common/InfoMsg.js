REQUIRE('chlk.models.common.Button');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.InfoMsg*/
    CLASS(
        'InfoMsg', [
            String, 'text',
            String, 'header',
            ArrayOf(chlk.models.common.Button), 'buttons'
        ]);
});
