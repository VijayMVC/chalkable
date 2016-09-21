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

        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.GroupId);
            this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
            this.listRequestId = SJX.fromValue(raw.listRequestId, String);
            this.name = SJX.fromValue(raw.name, String);
            this.withStudents = SJX.fromValue(raw.hasstudents, Boolean);
        }
    ]);
});