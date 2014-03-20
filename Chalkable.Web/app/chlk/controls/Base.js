REQUIRE('ria.mvc.DomControl');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.Base */
    CLASS(
        'Base', EXTENDS(ria.mvc.DomControl), [
            chlk.models.common.Role, function getUserRole(){
                return this.getContext().getSession().get('role');
            }
        ]);
});