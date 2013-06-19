REQUIRE('chlk.controllers.BaseController');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SysAdminController */
    CLASS(
        [ria.mvc.ControllerUri('index')],
        'SysAdminController', EXTENDS(chlk.controllers.BaseController), [
            VOID, function indexAction() {}
        ])
});
