NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.DisciplineHoverBoxItem*/
    CLASS(
        'DisciplineHoverBoxItem', [
            Number, 'count',

            [ria.serialize.SerializeProperty('disciplinename')],
            String, 'disciplineName'
        ]);
});
