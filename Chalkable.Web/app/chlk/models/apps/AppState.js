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
                BASE();
                this._states = {};
                this._states[chlk.models.apps.AppStateEnum.DRAFT] = "Not Submitted";
                this._states[chlk.models.apps.AppStateEnum.SUBMIT_FOR_APPROVE] = "Submitted for approve";
                this._states[chlk.models.apps.AppStateEnum.APPROVED] = "Approved";
                this._states[chlk.models.apps.AppStateEnum.REJECTED] = "Rejected";
                this._states[chlk.models.apps.AppStateEnum.LIVE] = "Live";
            },

            [[Boolean]],
            String, function toString(isLive_){
                var status = this._states[this.getStateId()];
                var currentStateId = this.getStateId();
                if (isLive_){
                    switch (currentStateId.valueOf()){
                        case chlk.models.apps.AppStateEnum.SUBMIT_FOR_APPROVE:
                            status = 'Live - Update awaiting approval';
                            break;
                        case chlk.models.apps.AppStateEnum.APPROVED:
                            status = 'Live - Update approved';
                            break;
                        case chlk.models.apps.AppStateEnum.REJECTED:
                            status = 'Live - Update approved';
                            break;
                        default :
                            status = 'Live';
                            break;
                    }
                }
                return status;
            },
            [[Number]],
            VOID, function deserialize(raw) {
                this.setStateId(new chlk.models.apps.AppStateEnum(Number(raw)));
            }
        ]);
});
