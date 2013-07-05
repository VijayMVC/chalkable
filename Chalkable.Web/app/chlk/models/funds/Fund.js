NAMESPACE('chlk.models.funds', function () {
    "use strict";
    /** @class chlk.models.funds.Fund*/
    CLASS(
        'Fund', [
            [ria.serialize.SerializeProperty('requestid')],
            Number, 'id',
            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'schoolId',
            [ria.serialize.SerializeProperty('schoolname')],
            Number, 'schoolName',
            [ria.serialize.SerializeProperty('ponumber')],
            Number, 'poNumber',
            Number, 'amount',
            //make enum
            [ria.serialize.SerializeProperty('statetype')],
            Number, 'stateType',
            String, 'name',
            String, 'terms',
            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName'
            //date created
        ]);
});
