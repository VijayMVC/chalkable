REQUIRE('chlk.models.common.HoverBox');
REQUIRE('chlk.models.discipline.StudentDisciplineHoverBoxItem');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /**@class chlk.models.discipline.StudentDisciplineHoverBox*/
    CLASS('StudentDisciplineHoverBox', EXTENDS(chlk.models.common.HoverBox),[

        ArrayOf(chlk.models.discipline.StudentDisciplineHoverBoxItem), 'hover',

        [ria.serialize.SerializeProperty('ispassing')],
        Boolean, 'passing',

        String, 'name'
    ]);
});
