REQUIRE('chlk.models.common.DisciplineHoverBoxItem');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.DisciplineHoverBox*/
    CLASS(
        'DisciplineHoverBox', EXTENDS(chlk.models.common.HoverBox), [
            ArrayOf(chlk.models.common.DisciplineHoverBoxItem), 'hover'
        ]);
});
