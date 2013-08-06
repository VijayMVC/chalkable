NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppAccess*/
    CLASS(
        'AppAccess', [
            [ria.serialize.SerializeProperty('haststudentmyapps')],
            Boolean, 'studentMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasteachermyapps')],
            Boolean, 'teacherMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasadminmyapps')],
            Boolean, 'adminMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasparentmypapps')],
            Boolean, 'parentMyAppsEnabled',
            [ria.serialize.SerializeProperty('hasmypappsview')],
            Boolean, 'myAppsViewVisible',
            [ria.serialize.SerializeProperty('canattach')],
            Boolean, 'attachEnabled',
            [ria.serialize.SerializeProperty('showingradeview')],
            Boolean, 'visibleInGradingView'
        ]);
});
