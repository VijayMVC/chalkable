REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.StudentForGroup*/

    CLASS('StudentForGroup', EXTENDS(chlk.models.people.User), [

        Boolean, 'assignedToGroup',

        OVERRIDE, VOID, function deserialize(raw){
            BASE(raw);
            this.assignedToGroup = SJX.fromValue(raw.assignedtogroup, Boolean);
        }
    ]);
});