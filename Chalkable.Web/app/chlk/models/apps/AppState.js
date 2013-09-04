REQUIRE('ria.serialize.IDeserializable');
NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppStateEnum*/

    ENUM(
        'AppStateEnum', {
            DRAFT: 1,
            SUBMIT_FOR_APPROVE: 2,
            APPROVED: 3,
            REJECTED: 4,
            LIVE: 5
        });


    /** @class chlk.models.apps.AppState*/
    CLASS(
        'AppState', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.apps.AppStateEnum, 'stateId',
            function $(){
                this._states= {};
                this._states[chlk.models.apps.AppStateEnum.DRAFT] = "Not Submitted";
                this._states[chlk.models.apps.AppStateEnum.SUBMIT_FOR_APPROVE] = "Submitted for approve";
                this._states[chlk.models.apps.AppStateEnum.APPROVED] = "Approved";
                this._states[chlk.models.apps.AppStateEnum.REJECTED] = "Rejected";
                this._states[chlk.models.apps.AppStateEnum.LIVE] = "Live";
            },

            [[Boolean]],
            String, function toString(hasLiveCopy_){
                var prefix = "";
                var currentStateId = this.getStateId();
                if (hasLiveCopy_){
                    if (currentStateId != chlk.models.apps.AppStateEnum.DRAFT && currentStateId != chlk.models.apps.AppStateEnum.LIVE)
                        prefix = "Live - ";
                }
                return prefix + this._states[this.getStateId()];
            },
            [[Number]],
            VOID, function deserialize(raw) {
                this.setStateId(new chlk.models.apps.AppStateEnum(Number(raw)));
            }
        ]);
});
