REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.standard.Standard');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.MiniQuizViewData*/
    CLASS(
        'MiniQuizViewData', [
            ArrayOf(chlk.models.standard.Standard), 'standards',

            chlk.models.id.StandardId, 'currentStandardId',

            [ria.serialize.SerializeProperty('applicationinfo')],
            chlk.models.apps.Application, 'applicationInfo',

            [ria.serialize.SerializeProperty('installedapplications')],
            ArrayOf(chlk.models.apps.Application), 'installedApplications',

            [ria.serialize.SerializeProperty('recommendedapplications')],
            ArrayOf(chlk.models.apps.Application), 'recommendedApplications',

            [ria.serialize.SerializeProperty('authorizationcode')],
            String, 'authorizationCode',

            String, 'standardIds'
        ]);
});
