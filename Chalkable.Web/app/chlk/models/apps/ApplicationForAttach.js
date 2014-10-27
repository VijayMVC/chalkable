REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.ApplicationForAttach*/
    CLASS(
        'ApplicationForAttach', EXTENDS(chlk.models.apps.Application), [

            [ria.serialize.SerializeProperty('notinstalledstudentscount')],
            Number, 'notInstalledStudentsCount',

            Boolean, function needToInstall(){
               return this.getNotInstalledStudentsCount() > 0;
            }
        ]);
});
