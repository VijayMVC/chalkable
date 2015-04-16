NAMESPACE('chlk.models.discipline', function () {
    "use strict";

    /** @class chlk.models.discipline.StudentDisciplineHoverBoxItem*/
    CLASS(
        'StudentDisciplineHoverBoxItem', [
            Number, 'value',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className'
        ]);
});
