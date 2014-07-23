REQUIRE('ria.mvc.Controller');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.Claim');

REQUIRE('chlk.activities.common.InfoMsgDialog');
REQUIRE('chlk.activities.lib.PendingActionDialog');

REQUIRE('chlk.models.common.InfoMsg');
REQUIRE('chlk.models.common.Button');
REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');
REQUIRE('chlk.lib.exception.NotAuthorizedException');
REQUIRE('chlk.lib.exception.AppErrorException');

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

           function $() {
               BASE();
               this.notAblePressSidebarButton = null;
           },

           Boolean, 'notAblePressSidebarButton',

           function closeCurrentActivity_(){
               return this.BackgroundCloseView(this.getView().getCurrent());
           },

           ria.async.Future, function validateResponse_() {
               var head, me = this;
               (head = new ria.async.Future)
                   .catchException(chlk.lib.exception.NotAuthorizedException, function (exception) {
                       document.location.href = WEB_SITE_ROOT;
                   })
                   .catchException(chlk.lib.exception.AppErrorException, function(exception){
                       return this.redirectToErrorPage_(exception.toString(), 'error', 'appError', []);
                   }, this)
                   .catchError(this.handleServerError, this);
               return head;
           },

           [[Object]],
           function handleServerError(error){
               if(error.getStatus && error.getStatus() == 500){
                    var response = JSON.parse(error.getResponse());
                    if(response.exceptiontype == 'ChalkableSisException')
                       return this.ShowMsgBox(response.message, 'oops',[{
                           text: Msg.GOT_IT.toUpperCase()
                       }]);
               }
               return this.redirectToErrorPage_(error.toString(), 'error', 'generalError', []);
           },

           function BackgroundNavigate(controller, action, args_) {
               this.context.getDefaultView().queueViewResult(this.Redirect(controller, action, args_));
               return null;
           },

           function BackgroundCloseView(activityClass) {
               this.context.getDefaultView().queueViewResult(this.CloseView(activityClass));
               return null;
           },

           [[String, String, String, Array]],
           function redirectToErrorPage_(error, controller, action, params){
               console.error(error);
               var state = this.context.getState();
               state.setController(controller);
               state.setAction(action);
               state.setParams(params);
               state.setPublic(false);
               this.context.stateUpdated();
               return ria.async.BREAK;
           },

           [[String, Function]],
           function getIdsList(ids, idClass){
               var result = ids ? ids.split(',').map(function(item){
                   return new idClass(item)
               }) : [];
               return result;
           },

           chlk.models.common.Role, function getCurrentRole(){
               return this.getContext().getSession().get(ChlkSessionConstants.USER_ROLE);
           },

           function ShadeLoader(){
               return this.ShadeView(chlk.activities.lib.PendingActionDialog, new ria.async.DeferredData(new Class()));
           },

           [[String, String, Array, String]],
           function ShowMsgBox(text_, header_, buttons_, clazz_) {
               var instance = new chlk.activities.common.InfoMsgDialog();
               var model = this.getMessageBoxModel_(text_, header_, buttons_, clazz_);
               this.view.shadeD(instance, ria.async.DeferredData(model));
           },

           [[String, String, Array, String]],
           chlk.models.common.InfoMsg, function getMessageBoxModel_(text_, header_, buttons_, clazz_){
               var buttons = [];
               if(buttons_){

                   var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                   buttons_.forEach(function(item){
                       buttons.push(serializer.deserialize(item, chlk.models.common.Button));
                   })
               }else{
                   buttons.push(new chlk.models.common.Button('Ok'));
               }
               return new chlk.models.common.InfoMsg(text_, header_, buttons, clazz_);
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


           Boolean, function isDemoSchool(){
               return this.getContext().getSession().get(ChlkSessionConstants.DEMO_SCHOOL, false);
           },

           [[Number]],
           VOID, function setNewNotificationCount_(value){
               this.getContext().getSession().set(ChlkSessionConstants.NEW_NOTIFICATIONS, value);
           },
           Number, function getNewNotificationCount_(){
               return this.getContext().getSession().get(ChlkSessionConstants.NEW_NOTIFICATIONS);
           },

           chlk.models.people.User, function getCurrentPerson(){
               return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON, null);
           },
           [chlk.models.people.User],
           VOID, function updateCurrentPerson(user){
               this.getContext().getSession().set(ChlkSessionConstants.CURRENT_PERSON, user);
           },

           chlk.models.schoolYear.MarkingPeriod, function getCurrentMarkingPeriod(){
               return this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD);
           },

           chlk.models.schoolYear.MarkingPeriod, function getNextMarkingPeriodId(){
               return this.getContext().getSession().get(ChlkSessionConstants.NEXT_MARKING_PERIOD);
           },

           chlk.models.id.SchoolYearId, function getCurrentSchoolYearId(){
               return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_SCHOOL_YEAR_ID);
           },

           ArrayOf(chlk.models.people.Claim), function getUserClaims_(){
                return this.getContext().getSession().get(ChlkSessionConstants.USER_CLAIMS);
           },
           [[chlk.models.people.UserPermissionEnum]],
           Boolean, function hasUserPermission_(userPermission){
                var claims = this.getUserClaims_();
                return claims && claims.length > 0
                    && claims.filter(function(claim){return claim.hasPermission(userPermission); }).length > 0;
           },

           OVERRIDE, ria.reflection.ReflectionMethod, function resolveRoleAction_(state){
               var ref = new ria.reflection.ReflectionClass(this.getClass());

               var role = this.getContext().getSession().get(ChlkSessionConstants.USER_ROLE);
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
               if (methodReflector.isAnnotatedWith(chlk.controllers.SidebarButton) && !this.isNotAblePressSidebarButton()){
                   var buttonCls = methodReflector.findAnnotation(chlk.controllers.SidebarButton)[0].clazz;
                   new ria.dom.Dom(SIDEBAR_CONTROLS_ID + ' .' + buttonCls).addClass(PRESSED_CLS);
               }
               this.setNotAblePressSidebarButton(false);
           }

   ])

});