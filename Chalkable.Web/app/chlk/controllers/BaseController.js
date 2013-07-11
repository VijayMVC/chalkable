REQUIRE('ria.mvc.Controller');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SidebarButton */
    ANNOTATION(
        [[String]],
        function SidebarButton(clazz) {});

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
           [[Function, ria.async.Future]],
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
                       this.BREAK(); // failed with exception, stop further processing

                       console.error(error.toString());
                       // todo: scoping !?
                       //me.view.showAlertBox(error.getMessage());
                   });

               return head;
           },

           [[ria.mvc.State]],
           OVERRIDE, VOID, function callAction_(state) {
              BASE(state);
               new ria.dom.Dom(SIDEBAR_CONTROLS_ID + ' .' + PRESSED_CLS).removeClass(PRESSED_CLS);
               var action = toCamelCase(state.getAction()) + ACTION_SUFFIX;
               var ref = ria.reflection.ReflectionFactory(this.getClass());
               var methodReflector = ref.getMethodReflector(action);
               if (methodReflector.isAnnotatedWith(chlk.controllers.SidebarButton)){
                   var buttonCls = methodReflector.findAnnotation(chlk.controllers.SidebarButton)[0].clazz;
                   new ria.dom.Dom(SIDEBAR_CONTROLS_ID + ' .' + buttonCls).addClass(PRESSED_CLS);
               }
           }

   ])

});