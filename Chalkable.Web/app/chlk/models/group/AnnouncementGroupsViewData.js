REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.group.GroupsListViewData');

REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.group.AnnouncementGroupsViewData*/
    CLASS(
        'AnnouncementGroupsViewData', EXTENDS(chlk.models.group.GroupsListViewData), IMPLEMENTS(ria.serialize.IDeserializable), [

            ArrayOf(chlk.models.id.GroupId), 'selected',
            String, 'controller',
            String, 'action',
            String, 'resultHidden',
            Object, 'hiddenParams',

            [[ArrayOf(chlk.models.group.Group), chlk.models.id.AnnouncementId, ArrayOf(chlk.models.id.GroupId), String, String, String, Object]],
            function $(groups_, announcementId_, selected_, controller_, action_, resultHidden_, hiddenParams_){
                BASE(groups_, announcementId_);
                if(selected_)
                    this.setSelected(selected_);
                if(controller_)
                    this.setController(controller_);
                if(action_)
                    this.setAction(action_);
                if(resultHidden_)
                    this.setResultHidden(resultHidden_);
                if(hiddenParams_)
                    this.setHiddenParams(hiddenParams_);
            },

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