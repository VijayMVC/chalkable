NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ChartItem*/
    CLASS(
        'ChartItem', [
            String, 'summary',

            Number, 'value',

            Number, 'avg'
        ]);
});
