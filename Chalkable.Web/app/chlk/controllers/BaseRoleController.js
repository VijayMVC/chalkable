REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.models.common.Logout');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.BaseRoleController */
   CLASS(
       'BaseRoleController', EXTENDS(chlk.controllers.BaseController), [

           [[chlk.models.common.Logout]],
           VOID, function refresh_(model) {
               var logoutTpl = new chlk.templates.common.Logout();
               var model = new chlk.models.common.Logout(model.getName());
               logoutTpl.assign(model);

               new ria.dom.Dom()
                   .fromHTML(logoutTpl.render())
                   .appendTo(new ria.dom.Dom('#logout-block').empty());
           }
   ])

});