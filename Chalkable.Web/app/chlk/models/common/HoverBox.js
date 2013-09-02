REQUIRE('chlk.models.common.HoverBoxItem');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.HoverBox*/
    CLASS(
        'HoverBox', [
            Number, 'title',

            ArrayOf(chlk.models.common.HoverBoxItem), 'hover'
        ]);
});
