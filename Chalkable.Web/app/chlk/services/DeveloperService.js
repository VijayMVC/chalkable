REQUIRE('chlk.services.BaseInfoService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.developer.DeveloperInfo');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DeveloperService*/
    CLASS(
        'DeveloperService', EXTENDS(chlk.services.BaseInfoService), [
            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(id) {
                return this.get('Developer/DeveloperInfo.json', chlk.models.developer.DeveloperInfo, {
                    developerId: id.valueOf()
                });
            },

            ria.async.Future, function getDevelopers() {
                return this.get('Developer/GetDevelopers.json', ArrayOf(chlk.models.developer.DeveloperInfo), {
                });
            },

            [[chlk.models.id.SchoolPersonId, String, String ,String]],
            ria.async.Future, function saveInfo(id, name, webSite, email) {
                return this
                    .post('Developer/UpdateInfo.json', chlk.models.developer.DeveloperInfo, {
                        developerId: id.valueOf(),
                        name: name,
                        webSiteLink: webSite,
                        email: email
                    })
                    .then(function(result){
                        this.userInfoChange.notify([result.getName()]);
                        return result;
                    }, this);
            },

//            [[chlk.models.id.SchoolPersonId, String]],
//            ria.async.Future, function updatePaymentInfo(id ,email){
//                //todo: response from server
//                var mdl = new chlk.models.developer.PayPalInfo();
//                mdl.setEmail(email);
//                return ria.async.DeferredData(mdl);
//            },

            [[chlk.models.id.SchoolPersonId, String, String]],
            ria.async.Future, function updatePaymentInfo(developerId, paypalAddress){
                return this.post('Developer/ChangePayPalLogin.json', Boolean, {
                    developerId: developerId.valueOf(),
                    paypalAddress: paypalAddress
                })
                .then(function(data){
                    if(data === true){
                        var developer = this.getCurrentDeveloper();
                        developer.setPayPalAddress(paypalAddress);
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_DEVELOPER, developer);
                        return new chlk.models.developer.PayPalInfo(paypalAddress);
                    }
                }, this);
            },

            chlk.models.developer.DeveloperInfo, function getCurrentDeveloper(){
                return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_DEVELOPER, null);
            }

        ])
});