REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    ASSET('~/assets/jade/activities/profile/ProfileTopBar.jade')();
    /**@class chlk.templates.profile.BaseProfileTpl*/
    CLASS('BaseProfileTpl', EXTENDS(chlk.templates.JadeTemplate),[

        [[String]],
        ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
            return [];
        }
    ]);
});