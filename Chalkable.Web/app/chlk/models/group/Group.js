REQUIRE('chlk.models.id.GroupId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.Group*/

    CLASS('Group', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.GroupId, 'id',
        chlk.models.id.AnnouncementId, 'announcementId',
        String, 'name',
        String, 'listRequestId',
        Boolean, 'withStudents',
        Number, 'studentCount',
        Boolean, 'newAdded',
        ArrayOf(chlk.models.people.User), 'students',

        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.GroupId);
            this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
            this.listRequestId = SJX.fromValue(raw.listRequestId, String);
            this.name = SJX.fromValue(raw.name, String);
            this.studentCount = SJX.fromValue(raw.studentcount, Number);
            this.withStudents = SJX.fromValue(!!raw.studentcount, Boolean);
            this.newAdded = SJX.fromValue(raw.newAdded, Boolean);
            this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.people.User);
        },

        [[String, chlk.models.id.GroupId]],
        function $(name_, id_){
            BASE();
            name_ && this.setName(name_);
            id_ && this.setId(id_);
        }
    ]);
});