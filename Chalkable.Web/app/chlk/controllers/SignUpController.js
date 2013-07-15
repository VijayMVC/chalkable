REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.SignUpService');
REQUIRE('chlk.activities.signup.SignUpListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SignUpController */
    CLASS(
        'SignUpController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SignUpService, 'signUpService',

        [chlk.controllers.SidebarButton('signups')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.signUpService
                .getSignUps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.signup.SignUpListPage, result);
        }

    ])
});
