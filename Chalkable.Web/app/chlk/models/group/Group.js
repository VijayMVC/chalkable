REQUIRE('chlk.models.id.GroupId');
REQUIRE('chlk.models.people.User');


NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.Group*/

    CLASS('Group', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.GroupId, 'id',
        String, 'name',
        Boolean, 'withStudents',

        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.GroupId);
            this.name = SJX.fromValue(raw.name, String);
            this.withStudents = SJX.fromValue(raw.hasstudents, Boolean);
        }
    ]);
});