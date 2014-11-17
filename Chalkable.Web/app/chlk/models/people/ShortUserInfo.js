REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.Role');
REQUIRE('chlk.models.common.AlertInfo');
REQUIRE('chlk.models.people.HealthCondition');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.ShortUserInfo*/
    CLASS(
        UNSAFE, 'ShortUserInfo', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.displayName = SJX.fromValue(raw.displayname, String);
                this.email = SJX.fromValue(raw.email, String);
                this.firstName = SJX.fromValue(raw.firstname, String);
                this.fullName = SJX.fromValue(raw.fullname, String);
                this.gender = SJX.fromValue(raw.gender, String);
                this.id = SJX.fromValue(raw.id, chlk.models.id.SchoolPersonId);
                this.lastName = SJX.fromValue(raw.lastname, String);
                this.password = SJX.fromValue(raw.password, String);
                this.roleName = SJX.fromValue(raw.roleName, String);
                this.genderFullText = SJX.fromValue(raw.genderFullText, String);
                this.salutation = SJX.fromValue(raw.salutation, String);
                this.withdrawn = SJX.fromValue(raw.iswithdrawn, Boolean);
                this.withMedicalAlert = SJX.fromValue(raw.hasmedicalalert, Boolean);
                this.allowedInetAccess = SJX.fromValue(raw.isallowedinetaccess, Boolean);
                this.specialInstructions = SJX.fromValue(raw.specialinstructions, String);
                this.spedStatus = SJX.fromValue(raw.spedstatus, String);
                this.healthConditions = SJX.fromArrayOfDeserializables(raw.healthconditions, chlk.models.people.HealthCondition);

                this.role = null;
                if(raw.role && raw.role.id != undefined) {
                    var objRole =  raw.role;
                    this.role = new chlk.models.people.Role.$create(objRole.id, objRole.name, objRole.namelowered, objRole.description);
                }
            },

            READONLY, String, 'displayName',
            READONLY, String, 'email',
            READONLY, String, 'firstName',
            READONLY, String, 'fullName',
            READONLY, String, 'gender',
            chlk.models.id.SchoolPersonId, 'id',
            READONLY, String, 'lastName',
            READONLY, String, 'password',
            READONLY, String, 'pictureUrl',
            READONLY, String, 'roleName',
            READONLY, chlk.models.people.Role, 'role',
            READONLY, String, 'genderFullText',
            READONLY, String, 'salutation',
            READONLY, Boolean, 'withdrawn',
            READONLY, Boolean, 'withMedicalAlert',
            READONLY, Boolean, 'allowedInetAccess',
            READONLY, String, 'specialInstructions',
            READONLY, String, 'spedStatus',
            READONLY, ArrayOf(chlk.models.people.HealthCondition), 'healthConditions',

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
                    || this.getSpecialInstructions() || this.getSpedStatus() && this.getSpedStatus() != 'Inactive';
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
                    res.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.SPECIAL_INSTRUCTIONS_ALERT, this.getSpecialInstructions()));
                if(this.getSpedStatus() && this.getSpedStatus() != 'Inactive')
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
