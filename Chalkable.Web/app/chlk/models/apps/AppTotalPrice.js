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

            Boolean, 'empty',

            [[Number, Number, Number]],
            function $create(price, totalPrice, totalPersonCount){
                BASE();
                this.setPrice(price);
                this.setTotalPrice(totalPrice);
                this.setTotalPersonsCount(totalPersonCount);
                this.setEmpty(false);
            },

            function $createEmpty(){
                BASE();
                this.setPrice(0);
                this.setTotalPrice(0);
                this.setTotalPersonsCount(0);
                this.setEmpty(true);
            }
        ]);
});