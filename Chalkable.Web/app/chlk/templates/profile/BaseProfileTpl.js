REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    ASSET('~/assets/jade/activities/profile/ProfileTopBar.jade')();
    /**@class chlk.templates.profile.BaseProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.profile.BaseProfileViewData)],
        'BaseProfileTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.common.RoleEnum, 'currentRoleId',

            Boolean, function isAdmin(){return this.getModel().isAdmin();},

            [[String]],
            ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){ return []; }
    ]);
});