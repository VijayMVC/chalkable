NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppAccess*/
    CLASS(
        'AppAccess', [
            [ria.serialize.SerializeProperty('haststudentmyapps')],
            Boolean, 'hasStudentMyApps',
            [ria.serialize.SerializeProperty('hasteachermyapps')],
            Boolean, 'hasTeacherMyApps',
            [ria.serialize.SerializeProperty('hasadminmyapps')],
            Boolean, 'hasAdminMyApps',
            [ria.serialize.SerializeProperty('hasparentmypapps')],
            Boolean, 'hasParentMyApps',
            [ria.serialize.SerializeProperty('hasmypappsview')],
            Boolean, 'hasMyAppsView',
            [ria.serialize.SerializeProperty('canattach')],
            Boolean, 'canAttach',
            [ria.serialize.SerializeProperty('showingradeview')],
            Boolean, 'showInGradeView'
        ]);
});
