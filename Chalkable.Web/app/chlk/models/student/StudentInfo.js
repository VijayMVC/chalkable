REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.people.HealthCondition');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        [ria.serialize.SerializeProperty('gradelevel')],
        chlk.models.grading.GradeLevel, 'gradeLevel',
        ArrayOf(chlk.models.people.User), 'parents',


        [ria.serialize.SerializeProperty('healthconditions')],
        ArrayOf(chlk.models.people.HealthCondition), 'healthConditions',


        OVERRIDE, chlk.models.common.Alerts, function getAlertsInfo(){
            var res = BASE().getAlerts();
            var healthConditions = this.getHealthConditions();
            if(healthConditions && healthConditions.length > 0){
                var commonNS = chlk.models.common;
                res = res.filter(function (item){
                    return item.getAlertType() != commonNS.AlertTypeEnum.MEDICAL_ALERT
                });
                for(var i = 0; i < healthConditions.length; i++){
                    if(healthConditions[i].isAlert())
                        res.push(new commonNS.AlertInfo(commonNS.AlertTypeEnum.MEDICAL_ALERT, healthConditions[i].getDescription()));
                }
            }
            return new chlk.models.common.Alerts(res);
        }
    ]);
});