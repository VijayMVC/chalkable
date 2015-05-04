REQUIRE('chlk.models.people.User');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.people.Schedule*/
    CLASS(
        'Schedule', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable), [

            Number, 'classesNumber',
            //chlk.models.people.ShortUserInfo, 'shortPersonInfo',

            OVERRIDE, VOID, function deserialize(raw){

                BASE(raw.shortpersoninfo);
                this.classesNumber = SJX.fromValue(raw.classesnumber, Number);
//                this.short
//                this.shortPersonInfo = SJX.fromDeserializable(raw.shortpersoninfo, chlk.models.people.ShortUserInfo);
            }
        ]);
});
