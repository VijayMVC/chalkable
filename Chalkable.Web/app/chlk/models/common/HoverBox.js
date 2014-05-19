REQUIRE('chlk.models.common.HoverBoxItem');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.HoverBox*/
    CLASS(
        GENERIC('THoverItem'),
        'HoverBox', [

            Number, 'title',
            ArrayOf(THoverItem), 'hover'
        ]);
});
