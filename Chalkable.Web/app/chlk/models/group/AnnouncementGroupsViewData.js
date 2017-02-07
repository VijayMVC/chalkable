NAMESPACE('chlk.models.group', function(){

    /** @class chlk.models.group.AnnouncementGroupsViewData*/
    CLASS(
        'AnnouncementGroupsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            Object, 'selected',

            VOID, function deserialize(raw){
                this.selected = raw.selected;
            }
        ]);
});