NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.model.apps.AppPrice*/
    CLASS(
        'AppPrice', [
            Number, 'price',
            [ria.serialize.SerializeProperty('priceperclass')],
            Number, 'pricePerClass',
            [ria.serialize.SerializeProperty('priceperschool')],
            Number, 'pricePerSchool'
        ]);
});