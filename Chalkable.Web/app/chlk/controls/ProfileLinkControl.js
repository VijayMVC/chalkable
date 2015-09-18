REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ProfileLinkControl */
    CLASS(
        'ProfileLinkControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/profile-link.jade')(this);
            },

            function hasAccess(){
                var claims = this.getContext().getSession().get(ChlkSessionConstants.USER_CLAIMS);
//                return claims && claims.length > 0
//                    && claims.filter(function(claim){return claim.hasPermission(chlk.models.people.UserPermissionEnum.MAINTAIN_PERSON); }).length > 0;
                return true;
            }
        ]);
});