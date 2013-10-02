REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.people.UserProfileModel');

NAMESPACE('chlk.templates.profile', function(){
   "use strict";

    /**@class chlk.templates.profile.SchoolPersonProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.people.UserProfileModel)],
        'SchoolPersonProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

        [[String]],
        OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
            return this.getModel().getActionLinksData(); // todo : rewrite this method
        }
    ]);
});