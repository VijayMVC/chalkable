NAMESPACE('chlk.models.dbmaintenance', function () {
    "use strict";
    /** @class chlk.models.dbmaintenance.DbBackup */
    CLASS(
        'DbBackup', [
            String, 'ticks',
            chlk.models.common.ChlkDate, 'created',
            [ria.serialize.SerializeProperty('hasmaster')],
            Boolean, 'hasMaster',
            [ria.serialize.SerializeProperty('hasschooltemplate')],
            Boolean, 'hasSchoolTemplate',
            [ria.serialize.SerializeProperty('schoolcount')],
            Number, 'schoolCount'
        ]);
});