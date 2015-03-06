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
REQUIRE('chlk.lib.exception.NoClassAnnouncementTypeException');
REQUIRE('chlk.lib.exception.AppErrorException');
REQUIRE('chlk.lib.exception.InvalidPictureException');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SidebarButton */
    ANNOTATION(
        [[String]],
        function SidebarButton(clazz) {});

    /** @class chlk.controllers.AccessForRoles */
    ANNOTATION(
        [[ArrayOf(chlk.models.common.RoleEnum)]],
        function AccessForRoles(roles){});

    /** @class chlk.controllers.Permissions */
    ANNOTATION(
        [[Array]],
        function Permissions(permissions){});

    /** @class chlk.controllers.StudyCenterEnabled*/
    ANNOTATION(
        function StudyCenterEnabled(){});


    function toCamelCase(str) {
        return str.replace(/(\-[a-z])/g, function($1){
            return $1.substring(1).toUpperCase();
        });
    }

    /** @class chlk.controllers.MissingPermissionException */
    EXCEPTION(
        'MissingPermissionException', [
            READONLY, Array, 'permissions',

            [[Array, Object]],
            function $(permissions, inner_) {
                BASE('Permissions ' + JSON.stringify(permissions) + ' is required.', inner_);
                this.permissions = permissions;
            }
        ]);

    /** @class chlk.controllers.StudyCenterDisabledException*/
    EXCEPTION(
        'StudyCenterDisabledException', [
            [[Object]],
            function $(inner_) {
                BASE('Required study center access', inner_);
            }
        ]);

    var PRESSED_CLS = 'active';
    var ACTION_SUFFIX = 'Action';
    var SIDEBAR_CONTROLS_ID = '#sidebar';

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
                   .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(exception){
                       return this.redirectToErrorPage_(exception.toString(), 'error', 'createAnnouncementError', []);
                   }, this)
                   .catchException(chlk.lib.exception.InvalidPictureException, function(exception){
                       return this.ShowMsgBox('You need to upload valid picture for you app', 'Error', [{
                           text: 'Ok'
                       }], 'center'), null;
                   }, this)
                   .catchError(this.handleServerError, this);
               return head;
           },

           [[Object]],
           function handleServerError(error){
               setTimeout(function(){
                   this.BackgroundCloseView(chlk.activities.lib.PendingActionDialog);
               }.bind(this), 1);

               if(error.getStatus && error.getStatus() == 500){
                    var response = JSON.parse(error.getResponse());
                    if(response.exceptiontype == 'ChalkableSisException'){
                        return this.ShowMsgBox(response.message, 'oops',[{
                                text: Msg.GOT_IT.toUpperCase()
                            }])
                            .thenBreak();
                    }
                    if(response.exceptiontype == 'NoAnnouncementException')
                       return this.redirectToErrorPage_(error.toString(), 'error', 'viewAnnouncementError', []);
                    if(response.exceptiontype == 'NoClassAnnouncementTypeException')
                       return this.redirectToErrorPage_(error.toString(), 'error', 'createAnnouncementError', []);
               }
               return this.redirectToErrorPage_(error.toString(), 'error', 'error404', []);
           },

           function BackgroundNavigate(controller, action, args_) {
               this.context.getDefaultView()
                   .queueViewResult(this.Redirect(controller, action, args_));
               return null;
           },

           function BackgroundCloseView(activityClass) {
               this.context.getDefaultView().queueViewResult(this.CloseView(activityClass));
               return null;
           },

           [[String, String, String, Array]],
           function redirectToErrorPage_(error, controller, action, params){
               console.error(error);
               return this.redirectToPage_(controller, action, params);
           },

           [[String, String, Array]],
           function redirectToPage_(controller, action, params){
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

           [[String, ArrayOf(String)]],
           Boolean, function isValidFileExtension(url, formats){
               var ext = url.split(".");
               if( ext.length === 1 || ( ext[0] === "" && ext.length === 2 ) ) {
                   ext = "";
               }
               else{
                   ext = ext.pop();
               }

               ext = ext.toLowerCase();
               var validExtensions = formats || [];

               var isValid = false;

               for (var i = 0; i < validExtensions.length; ++i){
                   var item = validExtensions[i];
                   if (item.toLowerCase() == ext) {
                       isValid = true;
                       break;
                   }
               }
               return isValid;
           },

           [[String, String, Array, String, Boolean]],
           ria.async.Future, function ShowMsgBox(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_) {
               var model = this.getMessageBoxModel_(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_);
               return this.view.showModal(chlk.activities.common.InfoMsgDialog, model);
           },

           [[String, String, Array, String, Boolean, String, Object]],
           chlk.models.common.InfoMsg, function getMessageBoxModel_(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_){
               var buttons = [];
               if(buttons_){
                   var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                   buttons_.forEach(function(item){
                       buttons.push(serializer.deserialize(item, chlk.models.common.Button));
                   });
               }else{
                   buttons.push(new chlk.models.common.Button('Ok'));
               }
               return new chlk.models.common.InfoMsg(text_, header_, buttons, clazz_, isHtmlText_, inputType_, inputAttrs_);
           },

           ria.async.Future, function ShowAlertBox(text, header_) {
               return this.ShowMsgBox(text, header_)
                   .then(function (activity) { return null; });
           },

           ria.async.Future, function ShowPromptBox(text, header_, inputValue_, inputAttrs_, inputType_) {
               return this.ShowMsgBox(text, header_, [{text: 'OK', clazz: 'blue-button', value: 'ok'}, {text: 'Cancel'}], null, false, inputType_ || 'text', inputValue_, inputAttrs_)
                   .then(function (mrResult) {
                       if (!mrResult)
                            return ria.async.BREAK;

                       return mrResult;
                   });
           },

           ria.async.Future, function ShowConfirmBox(text, header_, buttonText_, buttonClass_) {
               return this.ShowMsgBox(text, header_, [{text: buttonText_ || 'OK', clazz: buttonClass_ || 'blue-button', value: 'ok'}, {text: 'Cancel'}])
                   .then(function (mrResult) {
                       if (!mrResult)
                           return ria.async.BREAK;

                       return mrResult;
                   });
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

                   if (!this.checkUserRoles_(method, role))
                        throw new ria.mvc.MvcException('Controller ' + ref.getName() + ' has no method ' + method.getName()
                           + ' available for role ' + role.getRoleName());
               }

               var missingPermissions = this.checkPermissions_(method);
               if (missingPermissions.length) {
                   throw new chlk.controllers.MissingPermissionException(missingPermissions);
               }

               var isStudyCenterEnabled = this.getContext().getSession().get(ChlkSessionConstants.STUDY_CENTER_ENABLED);
               if(!this.checkStudyCenterEnabled_(method,isStudyCenterEnabled))
                   throw new chlk.controllers.StudyCenterDisabledException();

               return method;
           },

           OVERRIDE, ria.serialize.ISerializer, function initSerializer_(){
               return new chlk.lib.serialize.ChlkJsonSerializer();
           },

           [[ria.reflection.ReflectionMethod]],
           Boolean, function checkUserRoles_(method, role) {
               var annotations = method.findAnnotation(chlk.controllers.AccessForRoles);
               return !annotations.length || annotations
                   .reduce(function (prev, annotation) { return prev.concat(annotation.roles); }, [])
                   .indexOf(role.getRoleId()) >= 0;
           },

           [[ria.reflection.ReflectionMethod]],
           Array, function checkPermissions_(method) {
               var userHasPermission = this.hasUserPermission_, permArray;
               return method.findAnnotation(chlk.controllers.Permissions)
                   .reduce(function (prev, annotation) {
                       return prev.concat(annotation.permissions);
                   }, []).filter(function (userPermission) {
                       if(Array.isArray(userPermission)){
                           permArray = userPermission.filter(function(item){
                               return userHasPermission(item);
                           });
                           return !permArray.length;
                       }
                       return !userHasPermission(userPermission);
                   });
           },

           [[ria.reflection.ReflectionMethod, Boolean]],
           Boolean, function checkStudyCenterEnabled_(method, isStudyCenterEnabled){
               var annotations = method.findAnnotation(chlk.controllers.StudyCenterEnabled);
               return !annotations.length || isStudyCenterEnabled == true;
           },

           OVERRIDE, VOID, function postDispatchAction_() {
               BASE();

               var state = this.context.getState(),
                   $sidebar = ria.dom.Dom(SIDEBAR_CONTROLS_ID);

               $sidebar.find('A.' + PRESSED_CLS).removeClass(PRESSED_CLS);

               var methodReflector = this.resolveRoleAction_(state);
               if (methodReflector.isAnnotatedWith(chlk.controllers.SidebarButton) && !this.isNotAblePressSidebarButton()){
                   var buttonCls = methodReflector.findAnnotation(chlk.controllers.SidebarButton)[0].clazz;
                   $sidebar.find('A.' + buttonCls).addClass(PRESSED_CLS);
               }

               this.setNotAblePressSidebarButton(false);
           },


           [[ria.mvc.State]],
           OVERRIDE, VOID, function callAction_(state) {
               try {
                   BASE(state);
               } catch (e) {
                   if (e instanceof chlk.controllers.MissingPermissionException) {
                       state.setDispatched(false);
                       state.setController('error');
                       state.setAction('permissions');
                       state.setParams([e.getPermissions()]);
                       return;
                   }
                   if(e instanceof chlk.controllers.StudyCenterDisabledException){
                       state.setDispatched(false);
                       state.setController('error');
                       state.setAction('studyCenterAccess');
                       state.setParams([]);
                       return;
                   }
                   throw e;
               }
           }
   ])

});