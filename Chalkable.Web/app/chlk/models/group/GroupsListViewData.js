REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.GroupsListViewData*/

    CLASS('GroupsListViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.group.Group), 'groups',

        chlk.models.id.AnnouncementId, 'announcementId',

        String, 'listRequestId',

        VOID, function deserialize(raw){
            this.groups = SJX.fromArrayOfDeserializables(raw.groups, chlk.models.group.Group);
        },

        [[String, ArrayOf(chlk.models.group.Group), chlk.models.id.AnnouncementId]],
        function $(listRequestId_, groups_, announcementId_){
            BASE();
            if(groups_)
                this.setGroups(groups_);
            if(listRequestId_)
                this.setListRequestId(listRequestId_);
            if(announcementId_)
                this.setAnnouncementId(announcementId_);
        }
    ]);
});