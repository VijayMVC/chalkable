REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    ASSET('~/assets/jade/activities/profile/ProfileTopBar.jade')();
    /**@class chlk.templates.profile.BaseProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.profile.BaseProfileViewData)],
        'BaseProfileTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.common.RoleEnum, 'currentRoleId',

            Boolean, function isAdmin(){return this.getModel().isAdmin();},

            [[String]],
            ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){ return []; },

            chlk.models.common.ActionLinkModel, function createActionLinkModel_(controller, action, title
                , pressedAction_, args_, disabled_){
                return new chlk.models.common.ActionLinkModel(controller, action, title
                    , pressedAction_ && pressedAction_ == action, args_, null, disabled_);
            }
    ]);
});