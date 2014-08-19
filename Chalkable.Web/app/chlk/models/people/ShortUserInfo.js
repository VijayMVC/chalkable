REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.Role');
REQUIRE('chlk.models.common.AlertInfo');
REQUIRE('chlk.models.people.HealthCondition');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.ShortUserInfo*/
    CLASS(
        'ShortUserInfo', [

            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName',

            String, 'email',

            [ria.serialize.SerializeProperty('firstname')],
            String, 'firstName',

            [ria.serialize.SerializeProperty('fullname')],
            String, 'fullName',

            String, 'gender',

            chlk.models.id.SchoolPersonId, 'id',

            [ria.serialize.SerializeProperty('lastname')],
            String, 'lastName',

            String, 'password',

            String, 'pictureUrl',

            String, 'roleName',

            [ria.serialize.SerializeProperty('role')],
            chlk.models.people.Role, 'role',

            String, 'genderFullText',

            String, 'salutation',

            [ria.serialize.SerializeProperty('hasmedicalalert')],
            Boolean, 'withMedicalAlert',

            [ria.serialize.SerializeProperty('isallowedinetaccess')],
            Boolean, 'allowedInetAccess',

            [ria.serialize.SerializeProperty('specialinstructions')],
            String, 'specialInstructions',

            [ria.serialize.SerializeProperty('spedstatus')],
            String, 'spedStatus',


            [ria.serialize.SerializeProperty('healthconditions')],
            ArrayOf(chlk.models.people.HealthCondition), 'healthConditions',

            [[Array]],
            VOID, function addMedicalAlerts_(alerts){
                var healthConditions = this.getHealthConditions();
                var commonNS = chlk.models.common;
                if(healthConditions && healthConditions.length > 0){
                    for(var i = 0; i < healthConditions.length; i++){
                        if(healthConditions[i].isAlert()){
                            var description = healthConditions[i].getDescription() || healthConditions[i].getName();
                            alerts.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.MEDICAL_ALERT, description));
                        }
                    }
                }
                else alerts.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.MEDICAL_ALERT, Msg.Alert_Medical_text))
            },

            Boolean, function showAlerts(){
                var res = this.isWithMedicalAlert() || this.isAllowedInetAccess()
                    || this.getSpecialInstructions() || this.getSpedStatus();
                return !!res;
            },

            READONLY, chlk.models.common.Alerts, 'alertsInfo',
            chlk.models.common.Alerts, function getAlertsInfo(){
                var res = [];
                var commonNS = chlk.models.common;
                if (this.isAllowedInetAccess())
                    res.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.INTERNET_ACCESS_ALERT, Msg.Alert_Internet_access_text));
                if(this.isWithMedicalAlert())
                   // res.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.MEDICAL_ALERT, Msg.Alert_Medical_text));
                    this.addMedicalAlerts_(res);
                if(this.getSpecialInstructions())
                    res.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.SPECIAL_INSTRUCTIONS_ALERT, Msg.Alert_Special_text));
                if(this.getSpedStatus())
                    res.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.SPED_STATUS_ALERT, Msg.Alert_Sped_text));
                return new chlk.models.common.Alerts(res);
            },

            [[String, String, chlk.models.id.SchoolPersonId]],
            function $(firstName_, lastName_, id_){
                BASE();
                if(firstName_)
                    this.setFirstName(firstName_);
                if(lastName_)
                    this.setLastName(lastName_);
                if(id_)
                    this.setId(id_);
            }

        ]);
});
