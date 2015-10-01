REQUIRE('chlk.models.common.HoverBoxItem');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.CommonHoverBox*/
    CLASS(
        'CommonHoverBox', EXTENDS(chlk.models.common.HoverBox.OF(chlk.models.common.HoverBoxItem)), [
//            ArrayOf(chlk.models.common.HoverBoxItem), 'hover'
        ]);
});
