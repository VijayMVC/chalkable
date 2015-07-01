NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementType*/
    CLASS(
        'AnnouncementType', [

            function $(){
                BASE();
                this._description = null;
            },

            [ria.serialize.SerializeProperty('cancreate')],
            Boolean, 'canCreate',
            String, 'description',
            [[String]],
            VOID, function setDescription(description){
                this._description = description;
            },
            String, function getDescription(){
                return this._description || this.getName()
            },

            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId',

            [ria.serialize.SerializeProperty('issystem')],
            Boolean, 'system',
            String, 'name'
        ]);
});
