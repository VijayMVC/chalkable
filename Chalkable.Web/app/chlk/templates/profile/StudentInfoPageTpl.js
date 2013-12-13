REQUIRE('chlk.templates.profile.SchoolPersonInfoPageTpl');
REQUIRE('chlk.models.student.StudentProfileInfoViewData');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.StudentInfoPageTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/student-info-page.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileInfoViewData)],
        'StudentInfoPageTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentInfo)), [

            OVERRIDE, String, function render() {
                var res = BASE();
                var user = this.getModel().getUser();
                user.setAbleEdit(user.isAbleEdit()
                    && this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_ADDRESS)
                    && this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_PERSON));
                return res;
            }
        ])
});
