REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.group.GroupsListViewData');

REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.group.AnnouncementGroupsViewData*/
    CLASS(
        'AnnouncementGroupsViewData', EXTENDS(chlk.models.group.GroupsListViewData), IMPLEMENTS(ria.serialize.IDeserializable), [

            Object, 'selected',
            String, 'requestId',
            Object, 'hiddenParams',

            [[String, ArrayOf(chlk.models.group.Group), chlk.models.id.AnnouncementId, ArrayOf(chlk.models.id.GroupId), Object]],
            function $(requestId_, groups_, announcementId_, selected_, hiddenParams_){
                BASE(null, groups_, announcementId_);
                if(selected_)
                    this.setSelected(selected_);
                if(requestId_)
                    this.setRequestId(requestId_);
                if(hiddenParams_)
                    this.setHiddenParams(hiddenParams_);
            },

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.selected = raw.selected;
                this.requestId = SJX.fromValue(raw.requestId, String);
                this.hiddenParams = raw.hiddenParams || null;
            }
        ]);
});