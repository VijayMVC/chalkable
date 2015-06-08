REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.group.Group');


NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.AnnouncementGroupsViewData*/

    CLASS('AnnouncementGroupsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.group.Group), 'groups',
        ArrayOf(chlk.models.id.GroupId), 'groupIds',
        chlk.models.id.AnnouncementId, 'announcementId',

        VOID, function deserialize(raw){
            this.groups = SJX.fromArrayOfDeserializables(raw.groups, chlk.models.group.Group);
            this.groupIds = SJX.fromArrayOfValues(raw.groupIds, ArrayOf(chlk.models.id.GroupId));
            this.announcementId = SJX.fromValue(raw.announcementId, chlk.models.id.AnnouncementId);
        },

        [[ArrayOf(chlk.models.group.Group), ArrayOf(chlk.models.id.GroupId), chlk.models.id.AnnouncementId]],
        function $(groups_, groupIds_, announcementId_){
            BASE();
            if(groups_)
                this.setGroups(groups_);
            if(groupIds_)
                this.setGroupIds(groupIds_);
            if(announcementId_)
                this.setAnnouncementId(announcementId_);
        }
    ]);
});