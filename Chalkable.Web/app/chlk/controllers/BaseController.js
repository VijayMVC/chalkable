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
           },

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
           }

   ])

});