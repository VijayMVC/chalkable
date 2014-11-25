REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.id.BgTaskId');
NAMESPACE('chlk.models.district', function () {
    "use strict";

    /** @class chlk.models.district.District*/
    CLASS(
        'District', [
            chlk.models.id.DistrictId, 'id',
            String, 'name',
            [ria.serialize.SerializeProperty('sisurl')],
            String, 'sisUrl',
            [ria.serialize.SerializeProperty('dbname')],
            String, 'dbName',
            [ria.serialize.SerializeProperty('sisusername')],
            String, 'sisUserName',
            [ria.serialize.SerializeProperty('sispassword')],
            String, 'sisPassword',
            [ria.serialize.SerializeProperty('sissystemtype')],
            Number, 'sisSystemType',

            [ria.serialize.SerializeProperty('processingid')],
            chlk.models.id.BgTaskId, 'ProcessingId',

            [ria.serialize.SerializeProperty('processingcreated')],
            chlk.models.common.ChlkDate, 'ProcessingCreated',

            [ria.serialize.SerializeProperty('processingstarted')],
            chlk.models.common.ChlkDate, 'ProcessingStarted',


            [ria.serialize.SerializeProperty('completedid')],
            chlk.models.id.BgTaskId, 'CompletedId',

            [ria.serialize.SerializeProperty('completedcreated')],
            chlk.models.common.ChlkDate, 'CompletedCreated',

            [ria.serialize.SerializeProperty('completedstarted')],
            chlk.models.common.ChlkDate, 'CompletedStarted',

            [ria.serialize.SerializeProperty('completedcompleted')],
            chlk.models.common.ChlkDate, 'CompletedCompleted',


            [ria.serialize.SerializeProperty('failedid')],
            chlk.models.id.BgTaskId, 'FailedId',

            [ria.serialize.SerializeProperty('failedcreated')],
            chlk.models.common.ChlkDate, 'FailedCreated',

            [ria.serialize.SerializeProperty('failedstarted')],
            chlk.models.common.ChlkDate, 'FailedStarted',

            [ria.serialize.SerializeProperty('failedcompleted')],
            chlk.models.common.ChlkDate, 'FailedCompleted',


            [ria.serialize.SerializeProperty('canceledid')],
            chlk.models.id.BgTaskId, 'CanceledId',

            [ria.serialize.SerializeProperty('canceledcreated')],
            chlk.models.common.ChlkDate, 'CanceledCreated',

            [ria.serialize.SerializeProperty('canceledstarted')],
            chlk.models.common.ChlkDate, 'CanceledStarted',

            [ria.serialize.SerializeProperty('canceledcompleted')],
            chlk.models.common.ChlkDate, 'CanceledCompleted'

        ]);
});
