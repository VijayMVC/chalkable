REQUIRE('ria.mvc.Controller');

NAMESPACE('chlk.controllers', function (){


    /** @class chlk.controllers.BaseController */
   ABSTRACT, CLASS(
       'BaseController', EXTENDS(ria.mvc.Controller), [

   ])

});