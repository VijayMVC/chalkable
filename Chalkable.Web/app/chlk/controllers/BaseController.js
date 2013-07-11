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

    /** @class chlk.controllers.BaseController */
   ABSTRACT, CLASS(
       'BaseController', EXTENDS(ria.mvc.Controller), [
           /*[[Function, ria.async.Future]],
           VOID, function PushView(activityClass, data) {
               var instance = new activityClass;
               instance.refreshD(data);

               this.getView().push(instance);
           },
           VOID, function ShadeView(activityClass, data) {
               var instance = new activityClass;
               instance.refreshD(data);

               this.getView().shade(instance);
           },
           VOID, function UpdateView(activityClass, data, msg_) {

           },*/


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
               new ria.dom.Dom('#sidebar-controls .pressed').removeClass('pressed');
               var action = toCamelCase(state.getAction()) + 'Action';
               var ref = ria.reflection.ReflectionFactory(this.getClass());
               var method = ref.getMethodReflector(action);
               if (method.isAnnotatedWith(chlk.controllers.SidebarButton)){
                   var buttonCls = method.findAnnotation(chlk.controllers.SidebarButton)[0].clazz;
                   new ria.dom.Dom('#sidebar-controls .' + buttonCls).addClass('pressed');
               }
           }

   ])

});