REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.people', function(){
    "use strict";

    var SJX = ria.serialize.SJX;
    /**@class chlk.models.people.PersonApps*/
    CLASS(
        UNSAFE, 'PersonApps', EXTENDS(chlk.models.people.ShortUserInfo), [

            ArrayOf(chlk.models.apps.Application), 'applications',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw.person);
                this.applications = SJX.fromArrayOfDeserializables(raw.applications, chlk.models.apps.Application);
            }

        ]);
});