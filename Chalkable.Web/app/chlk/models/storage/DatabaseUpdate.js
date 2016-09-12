NAMESPACE('chlk.models.storage', function () {
    "use strict";
    /** @class chlk.models.storage.DatabaseUpdate*/
    CLASS(
        'DatabaseUpdate', [
           String, 'masterSql',
           String, 'schoolSql'
        ]);
});
