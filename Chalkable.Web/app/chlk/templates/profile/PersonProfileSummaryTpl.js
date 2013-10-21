REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.people.UserProfileSummaryViewData');

NAMESPACE('chlk.templates.profile', function(){
   "use strict";
    /**@class chlk.templates.profile.PersonProfileSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/SchoolPersonProfileSummaryView.jade')],
        [ria.templates.ModelBind(chlk.models.people.UserProfileSummaryViewData)],
        'PersonProfileSummaryTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.people.PersonSummary)),[

            String, function getStatusText(){
                var user = this.getUser();
                var roleId = user.getRole().getId();
                if(roleId == chlk.models.common.RoleEnum.TEACHER)
                    return user.getRoomName();
                return "";
            }
    ]);
});