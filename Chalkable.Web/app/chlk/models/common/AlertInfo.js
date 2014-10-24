NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.AlertTypeEnum*/
    ENUM(
        'AlertTypeEnum', {
            INTERNET_ACCESS_ALERT: 0,
            MEDICAL_ALERT: 1,
            SPECIAL_INSTRUCTIONS_ALERT: 2,
            SPED_STATUS_ALERT: 3
        });

    /** @class chlk.models.common.AlertInfo*/
    CLASS('AlertInfo', [

            chlk.models.common.AlertTypeEnum, 'alertType',
            String, 'description',

            [[chlk.models.common.AlertTypeEnum, String]],
            function $(alertType_, description_){
                BASE();
                if(alertType_){
                    this.setAlertType(alertType_);
                }
                if(description_)
                    this.setDescription(description_);
            }
        ]);

    /** @class chlk.models.common.Alerts*/
    CLASS('Alerts', [

        [[ArrayOf(chlk.models.common.AlertInfo)]],
        function $(alerts_){
            BASE();
            if(alerts_)
                this.setAlerts(alerts_);
        },

        [[String]],
        function $create(stringAlert){
            BASE();
            if(stringAlert){
                var data = JSON.parse(stringAlert);
                var alerts =(new chlk.lib.serialize.ChlkJsonSerializer())
                    .deserialize(data, ArrayOf(chlk.models.common.AlertInfo));
                this.setAlerts(alerts);
            }

        },

        ArrayOf(chlk.models.common.AlertInfo), 'alerts',

        READONLY, String, 'stringAlerts',
        String, function getStringAlerts(){
            var data = this.getAlerts().map(function(item){
                return {
                    alertType: item.getAlertType().valueOf(),
                    description: item.getDescription()
                };
            });
            return JSON.stringify(data);
        }
    ]);
});