REQUIRE('chlk.models.common.HoverBoxItem');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.CommonHoverBox*/
    CLASS(
        'CommonHoverBox', EXTENDS(chlk.models.common.HoverBox), [
            ArrayOf(chlk.models.common.HoverBoxItem), 'hover'
        ]);
});
