NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.DisciplineHoverBoxItem*/
    CLASS(
        'DisciplineHoverBoxItem', [
            Number, 'total',

            [ria.serialize.SerializeProperty('disciplinetype')],
            chlk.models.common.NameId, 'disciplineType'
        ]);
});
