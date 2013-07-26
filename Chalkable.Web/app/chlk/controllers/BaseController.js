REQUIRE('ria.mvc.Controller');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SidebarButton */
    ANNOTATION(
        [[String]],
        function SidebarButton(clazz) {});


    ANNOTATION(
        [[ArrayOf(chlk.models.common.RoleEnum)]],
        function AccessForRoles(roles){});

    function toCamelCase(str) {
        return str.replace(/(\-[a-z])/g, function($1){
            return $1.substring(1).toUpperCase();
        });
    }



    var PRESSED_CLS = 'pressed';
    var ACTION_SUFFIX = 'Action';
    var SIDEBAR_CONTROLS_ID = '#sidebar-controls';

    /** @class chlk.controllers.BaseController */
   ABSTRACT, CLASS(
       'BaseController', EXTENDS(ria.mvc.Controller), [

           ria.async.Future, function validateResponse_() {
               var head
                   , me = this;

               (head = new ria.async.Future)
                   .catchError(function (error) {
                       throw chlk.services.DataException('Failed to load data', error);
                   })
                   .then(function (data) {
                       // TODO: check response here
                       /*if (!data.isOkResponse())
                        throw chlk.services.DataException('Failed to load data: ' + $L(data.getErrorCode()));*/

                       return data;
                   })
                   .catchException(chlk.services.DataException, function (error) {
                       ria.async.BREAK; // failed with exception, stop further processing

                       console.error(error.toString());
                       // todo: scoping !?
                       //me.view.showAlertBox(error.getMessage());
                   });

               return head;
           },

           chlk.models.common.Role, function getCurrentRole(){
               return this.getContext().getSession().get('role');
           },

           [[chlk.models.common.RoleEnum]],
           Boolean, function userInRole(roleId){
               return this.getCurrentRole().getRoleId() == roleId;
           },


           OVERRIDE, ria.reflection.ReflectionMethod, function resolveRoleAction_(state){
               var ref = new ria.reflection.ReflectionClass(this.getClass());

               var role = this.getContext().getSession().get('role');
               var roleAction = toCamelCase(state.getAction()) + role.getRoleName() + 'Action';
               var method = ref.getMethodReflector(roleAction);

               if (!method){
                   method = BASE(state);
                   var accessForAnnotation = method.findAnnotation(chlk.controllers.AccessForRoles)[0];
                   if (accessForAnnotation){

                       var filteredRoles = accessForAnnotation.roles.filter(function (r) {
                           return r == role.getRoleId();
                       });

                       if (filteredRoles.length != 1){
                           throw new ria.mvc.MvcException('Controller ' + ref.getName() + ' has no method ' + method.getName()
                               + ' available for role ' + role.getRoleName());
                       }
                   }

               }
               return method;
           },

           OVERRIDE, VOID, function postDispatchAction_() {
               BASE();

               var state = this.context.getState();
               new ria.dom.Dom(SIDEBAR_CONTROLS_ID + ' .' + PRESSED_CLS).removeClass(PRESSED_CLS);
               var methodReflector = this.resolveRoleAction_(state);
               if (methodReflector.isAnnotatedWith(chlk.controllers.SidebarButton)){
                   var buttonCls = methodReflector.findAnnotation(chlk.controllers.SidebarButton)[0].clazz;
                   new ria.dom.Dom(SIDEBAR_CONTROLS_ID + ' .' + buttonCls).addClass(PRESSED_CLS);
               }
           }

   ])

});