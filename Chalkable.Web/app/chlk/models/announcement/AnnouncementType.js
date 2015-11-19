NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AnnouncementType*/
    CLASS(
        UNSAFE, 'AnnouncementType', IMPLEMENTS(ria.serialize.IDeserializable), [

            function $(){
                BASE();
                this._description = null;
            },

            Boolean, 'canCreate',

            String, 'description',

            [[String]],
            VOID, function setDescription(description){
                this._description = description;
            },
            String, function getDescription(){
                return this._description || this.getName()
            },

            Number, 'announcementTypeId',

            Boolean, 'system',

            String, 'name',

            VOID, function deserialize(raw){
                this.canCreate = SJX.fromValue(raw.cancreate, Boolean);
                this.description = SJX.fromValue(raw.description, String);
                this.announcementTypeId = SJX.fromValue(raw.announcementtypeid, Number);
                this.system = SJX.fromValue(raw.issystem, Boolean);
                this.name = SJX.fromValue(raw.name, String);
            }
        ]);
});
