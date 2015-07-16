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

            [ria.serialize.SerializeProperty('failcounter')],
            Number, 'failCounter',

            [ria.serialize.SerializeProperty('syncfrequency')],
            Number, 'syncFrequency',

            [ria.serialize.SerializeProperty('maxsyncfrequency')],
            Number, 'maxSyncFrequency',

            [ria.serialize.SerializeProperty('failDelta')],
            Number, 'failDelta',

            [ria.serialize.SerializeProperty('processingid')],
            chlk.models.id.BgTaskId, 'processingId',

            [ria.serialize.SerializeProperty('processingcreated')],
            chlk.models.common.ChlkDate, 'processingCreated',

            [ria.serialize.SerializeProperty('processingscheduled')],
            chlk.models.common.ChlkDate, 'processingScheduled',

            [ria.serialize.SerializeProperty('processingstarted')],
            chlk.models.common.ChlkDate, 'processingStarted',


            [ria.serialize.SerializeProperty('completedid')],
            chlk.models.id.BgTaskId, 'completedId',

            [ria.serialize.SerializeProperty('completedcreated')],
            chlk.models.common.ChlkDate, 'completedCreated',

            [ria.serialize.SerializeProperty('completedscheduled')],
            chlk.models.common.ChlkDate, 'completedScheduled',

            [ria.serialize.SerializeProperty('completedstarted')],
            chlk.models.common.ChlkDate, 'completedStarted',

            [ria.serialize.SerializeProperty('completedcompleted')],
            chlk.models.common.ChlkDate, 'completedCompleted',


            [ria.serialize.SerializeProperty('failedid')],
            chlk.models.id.BgTaskId, 'failedId',

            [ria.serialize.SerializeProperty('failedcreated')],
            chlk.models.common.ChlkDate, 'failedCreated',

            [ria.serialize.SerializeProperty('failedscheduled')],
            chlk.models.common.ChlkDate, 'failedScheduled',

            [ria.serialize.SerializeProperty('failedstarted')],
            chlk.models.common.ChlkDate, 'failedStarted',

            [ria.serialize.SerializeProperty('failedcompleted')],
            chlk.models.common.ChlkDate, 'failedCompleted',


            [ria.serialize.SerializeProperty('canceledid')],
            chlk.models.id.BgTaskId, 'canceledId',

            [ria.serialize.SerializeProperty('canceledcreated')],
            chlk.models.common.ChlkDate, 'canceledCreated',

            [ria.serialize.SerializeProperty('canceledscheduled')],
            chlk.models.common.ChlkDate, 'canceledScheduled',

            [ria.serialize.SerializeProperty('canceledstarted')],
            chlk.models.common.ChlkDate, 'canceledStarted',

            [ria.serialize.SerializeProperty('canceledcompleted')],
            chlk.models.common.ChlkDate, 'canceledCompleted',

            [ria.serialize.SerializeProperty('newid')],
            chlk.models.id.BgTaskId, 'newId',

            [ria.serialize.SerializeProperty('newcreated')],
            chlk.models.common.ChlkDate, 'newCreated',

            [ria.serialize.SerializeProperty('newscheduled')],
            chlk.models.common.ChlkDate, 'newScheduled'

        ]);
});
