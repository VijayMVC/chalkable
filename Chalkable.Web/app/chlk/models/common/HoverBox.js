REQUIRE('chlk.models.common.HoverBoxItem');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.HoverBox*/
    CLASS(
        GENERIC('THoverItem'),
        'HoverBox', [

            String, 'title',

            [ria.serialize.SerializeProperty('ispassing')],
            Boolean, 'passing',

            ArrayOf(THoverItem), 'hover'
        ]);
});
