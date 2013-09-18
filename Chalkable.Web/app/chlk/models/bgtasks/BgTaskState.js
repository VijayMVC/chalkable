REQUIRE('ria.serialize.IDeserializable');
NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskStateEnum*/
    ENUM(
        'BgTaskStateEnum', {
            CREATED: 0,
            PROCESSING: 1,
            PROCESSED: 2,
            CANCELED: 3,
            FAILED: 4
        });


    /** @class chlk.models.bgtasks.BgTaskState*/
    CLASS(
        'BgTaskState', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.bgtasks.BgTaskStateEnum, 'typeId',
            function $(){
                BASE();
                this._types = {};
                this._types[chlk.models.bgtasks.BgTaskStateEnum.CREATED] = "Created";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.PROCESSING] = "Processing";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.PROCESSED] = "Processed";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.CANCELED] = "Canceled";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.FAILED] = "Failed";
            },
            String, function toString(){
                return this._types[this.getTypeId()];
            },
            VOID, function deserialize(raw) {
                this.setTypeId(new chlk.models.bgtasks.BgTaskStateEnum(Number(raw)));
            }
        ]);
});
