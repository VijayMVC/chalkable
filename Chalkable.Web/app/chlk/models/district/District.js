NAMESPACE('chlk.models.district', function () {
    "use strict";
    /** @class chlk.models.district.District*/
    CLASS(
        'District', [
            Number, 'id',
            String, 'name',
            String, 'sisUrl',
            String, 'dbName',
            String, 'sisUserName',
            String, 'sisPassword',
            String, 'sisSystemType'
        ]);
});
