NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppTotalPrice*/
    CLASS(
        'AppTotalPrice', [
            Number, 'price',
            [ria.serialize.SerializeProperty('totalprice')],
            Number, 'totalPrice',

            [ria.serialize.SerializeProperty('totalpersonscount')],
            Number, 'totalPersonsCount',

            [[Number, Number, Number]],
            function $create(price, totalPrice, totalPersonCount){
                BASE();
                this.setPrice(price);
                this.setTotalPrice(totalPrice);
                this.setTotalPersonsCount(totalPersonCount);
            }

        ]);
});