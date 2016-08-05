NAMESPACE('chlk.models.storage', function () {
    "use strict";
    /** @class chlk.models.storage.Blob*/
    CLASS(
        'Blob', [
           String, 'name',
           String, 'uri',
           Number, 'size'
        ]);
});
