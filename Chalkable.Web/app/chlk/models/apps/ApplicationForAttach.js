REQUIRE('chlk.models.apps.Application');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.ApplicationForAttach*/

    var SJX = ria.serialize.SJX;

    CLASS(
        'ApplicationForAttach', EXTENDS(chlk.models.apps.Application), IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.notInstalledStudentsCount = SJX.fromValue(raw.notinstalledstudentscount, Number);
            },

            Number, 'notInstalledStudentsCount',

            Boolean, function needToInstall(){
               return this.getNotInstalledStudentsCount() > 0;
            }
        ]);
});
