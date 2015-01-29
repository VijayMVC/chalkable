NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.MiniQuizViewData*/
    CLASS(
        'MiniQuizViewData', [
            ArrayOf(String), 'ccStandardCodes',
            String, 'currentStandardCode',

            [ria.serialize.SerializeProperty('applicationinfo')],
            chlk.models.apps.Application, 'applicationInfo',

            [ria.serialize.SerializeProperty('installedapplications')],
            ArrayOf(chlk.models.apps.Application), 'installedApplications',

            [ria.serialize.SerializeProperty('recommendedapplications')],
            ArrayOf(chlk.models.apps.Application), 'recommendedApplications'
        ]);
});
