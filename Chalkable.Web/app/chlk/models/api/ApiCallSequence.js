REQUIRE('chlk.models.api.ApiListItem');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiCallSequence*/
    CLASS(
        'ApiCallSequence', [
            ArrayOf(chlk.models.api.ApiListItem), 'items',

            [[ArrayOf(chlk.models.api.ApiListItem)]],
            function $create(items){
                BASE();
                this.setItems(items);
            }
        ]);
});
