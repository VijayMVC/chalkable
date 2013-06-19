REQUIRE('ria.mvc.Controller');

NAMESPACE('chlk.controllers', function (){


    /** @class chlk.controllers.BaseController */
   ABSTRACT, CLASS(
       'BaseController', EXTENDS(ria.mvc.Controller), [
           [[Function, ria.async.Future]],
           VOID, function PushView(activityClass, data) {
               var instance = new activityClass;

               data.then(function (data) {
                   instance.refresh(data);
               });

               this.getView().push(instance);
           }

   ])

});