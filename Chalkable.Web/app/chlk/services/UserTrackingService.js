REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.UserTrackingService*/
    CLASS(
        'UserTrackingService', EXTENDS(chlk.services.BaseService), [

            [[String, String]],
            VOID, function openedAppFrom(appName, location){
                this.sendTrackEvent_("Launched App", {"From": location});
            },

            VOID, function tookMiniQuiz(){
                this.sendTrackEvent_("Took Mini Quiz", {});
            },

            VOID, function tookAssessment(){
                this.sendTrackEvent_("Took Assessment", {});
            },

            [[String, Object]],
            function sendTrackEvent_(name, ev){
                var mixpanel = window.mixpanel;
                if (mixpanel && mixpanel.track){
                    mixpanel.track(name, ev);
                }
            }
        ])
});