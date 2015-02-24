REQUIRE('ria.serialize.SJX');
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

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppState*/
    CLASS(
        UNSAFE, FINAL, 'AppState',IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.apps.AppStateEnum, 'stateId',

            READONLY, String, 'status',
            String, function getStatus(){
                return this._states[this.getStateId()];
            },

            [[chlk.models.apps.AppStateEnum]],
            function $(stateId_){
                BASE();
                this._states = {};
                this._states[chlk.models.apps.AppStateEnum.DRAFT] = "Not Submitted";
                this._states[chlk.models.apps.AppStateEnum.SUBMITTED_FOR_APPROVAL] = "Submitted for approval";
                this._states[chlk.models.apps.AppStateEnum.APPROVED] = "Approved";
                this._states[chlk.models.apps.AppStateEnum.REJECTED] = "Rejected";
                this._states[chlk.models.apps.AppStateEnum.LIVE] = "Live";
                if(stateId_)
                    this.setStateId(stateId_);
            },

            [[Boolean]],
            String, function toString(){
                return this.getStatus();
            },
            [[Object]],
            VOID, function deserialize(raw) {
                VALIDATE_ARG('raw', Number, raw);
                this.stateId = SJX.fromValue(Number(raw), chlk.models.apps.AppStateEnum);
            }
        ]);
});
