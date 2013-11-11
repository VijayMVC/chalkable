REQUIRE('ria.mvc.Controller');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.people.User');

REQUIRE('chlk.activities.common.InfoMsgDialog');
REQUIRE('chlk.activities.lib.PendingActionDialog');

REQUIRE('chlk.models.common.InfoMsg');
REQUIRE('chlk.models.common.Button');
REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');

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
   CLASS(ABSTRACT,
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
                       console.error(error.toString());

                       var state = this.context.getState();
                       state.setController('error');
                       state.setAction('generalError');
                       state.setParams([]);
                       state.setPublic(false);
                       this.context.stateUpdated();

                       // todo: scoping !?
                       //me.view.showAlertBox(error.getMessage());
                       return ria.async.BREAK; // failed with exception, stop further processing
                   }, this);

               return head;
           },

           [[String, Function]],
           function getIdsList(ids, idClass){
               var result = ids ? ids.split(',').map(function(item){
                   return new idClass(item)
               }) : [];
               return result;
           },

           chlk.models.common.Role, function getCurrentRole(){
               return this.getContext().getSession().get('role');
           },

           function ShadeLoader(){
               this.ShadeView(chlk.activities.lib.PendingActionDialog, new ria.async.DeferredData(new Class()));
           },

           [[String, String, Array, String]],
           function ShowMsgBox(text_, header_, buttons_, clazz_) {
               var instance = new chlk.activities.common.InfoMsgDialog();
               var buttons = [];
               if(buttons_){

                   var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                   buttons_.forEach(function(item){
                       buttons.push(serializer.deserialize(item, chlk.models.common.Button));
                   })
               }else{
                   buttons.push(new chlk.models.common.Button('Ok'));
               }
               var model = new chlk.models.common.InfoMsg(text_, header_, buttons, clazz_);
               this.view.shadeD(instance, ria.async.DeferredData(model));
           },

           [[chlk.models.common.RoleEnum]],
           Boolean, function userInRole(roleId){
               return this.getCurrentRole().getRoleId() == roleId;
           },

           Boolean, function userIsAdmin(){
               return this.userInRole(chlk.models.common.RoleEnum.ADMINEDIT) ||
                   this.userInRole(chlk.models.common.RoleEnum.ADMINGRADE) ||
                   this.userInRole(chlk.models.common.RoleEnum.ADMINVIEW);
           },

           chlk.models.people.User, function getCurrentPerson(){
               return this.getContext().getSession().get('currentPerson');
           },
           [chlk.models.people.User],
           VOID, function updateCurrentPerson(user){
               this.getContext().getSession().set('currentPerson', user);
           },

           chlk.models.schoolYear.MarkingPeriod, function getCurrentMarkingPeriod(){
               return this.getContext().getSession().get('markingPeriod');
           },
           chlk.models.id.SchoolYearId, function getCurrentSchoolYearId(){
               return this.getContext().getSession().get('currentSchoolYearId');
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

           OVERRIDE, ria.serialize.ISerializer, function initSerializer_(){
              return new chlk.lib.serialize.ChlkJsonSerializer();
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
           },

           [[ria.mvc.IActivity]],
           OVERRIDE, function prepareActivity(activity){
              activity.setRole(this.getCurrentRole());
           }

   ])

});