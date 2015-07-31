REQUIRE('chlk.models.group.Group');


NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.GroupsListViewData*/

    CLASS('GroupsListViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.group.Group), 'groups',

        VOID, function deserialize(raw){
            this.groups = SJX.fromArrayOfDeserializables(raw.groups, chlk.models.group.Group);
        },

        [[ArrayOf(chlk.models.group.Group)]],
        function $(groups_){
            BASE();
            if(groups_)
                this.setGroups(groups_);
        }
    ]);
});