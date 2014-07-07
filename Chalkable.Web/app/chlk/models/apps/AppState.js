REQUIRE('ria.serialize.IDeserializable');
NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppStateEnum*/

    ENUM(
        'AppStateEnum', {
            DRAFT: 1,
            SUBMITTED_FOR_APPROVAL: 2,
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
                this._states[chlk.models.apps.AppStateEnum.SUBMITTED_FOR_APPROVAL] = "Submitted for approval";
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
                        case chlk.models.apps.AppStateEnum.SUBMITTED_FOR_APPROVAL.valueOf():
                            status = 'Live - Update awaiting approval';
                            break;
                        case chlk.models.apps.AppStateEnum.APPROVED.valueOf():
                            status = 'Live - Update approved';
                            break;
                        case chlk.models.apps.AppStateEnum.REJECTED.valueOf():
                            status = 'Live - Update approved';
                            break;
                        default :
                            status = 'Your app is live in the Chalkable App Store';
                            break;
                    }
                }
                return status;
            },
            [[Object]],
            VOID, function deserialize(raw) {
                VALIDATE_ARG('raw', Number, raw);
                this.setStateId(new chlk.models.apps.AppStateEnum(Number(raw)));
            }
        ]);
});
