REQUIRE('chlk.models.id.GroupId');
REQUIRE('chlk.models.people.User');


NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.Group*/

    CLASS('Group', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.GroupId, 'id',
        String, 'name',
        ArrayOf(chlk.models.people.User), 'students',

        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.GradeId);
            this.name = SJX.fromValue(raw.name, String);
            this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.people.User);
        }
    ]);
});