REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.group.GroupsListViewData');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.AnnouncementGroupsViewData*/

    CLASS('AnnouncementGroupsViewData', EXTENDS(chlk.models.group.GroupsListViewData), IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.id.GroupId), 'selected',
        String, 'controller',
        String, 'action',
        String, 'resultHidden',
        Object, 'hiddenParams',

        OVERRIDE, VOID, function deserialize(raw){
            BASE(raw);
            this.selected = SJX.fromArrayOfValues(raw.selected, chlk.models.id.GroupId) || [];
            this.controller = SJX.fromValue(raw.controller, String);
            this.action = SJX.fromValue(raw.action, String);
            this.resultHidden = SJX.fromValue(raw.resultHidden, String);
            this.hiddenParams = raw.hiddenParams || null;
        }
    ]);
});