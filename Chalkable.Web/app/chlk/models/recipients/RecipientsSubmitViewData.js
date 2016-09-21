REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.recipients.RecipientsSubmitViewData*/
    CLASS(
        UNSAFE, 'RecipientsSubmitViewData',
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.groupIds = SJX.fromValue(raw.groupIds, String);
                this.studentIds = SJX.fromValue(raw.studentIds, String);
                this.requestId = SJX.fromValue(raw.requestId, String);
                this.name = SJX.fromValue(raw.name, String);
                if(raw.selectedItems)
                    this.selectedItems = JSON.parse(SJX.fromValue(raw.selectedItems, String));
            },

            String, 'groupIds',
            String, 'studentIds',
            String, 'requestId',
            String, 'name',
            Object, 'selectedItems'
        ]);
});
