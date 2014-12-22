REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.activities.lib', function () {

    /** @class chlk.activities.lib.PageClass */
    ANNOTATION(
        [[String]],
        function PageClass(clazz) {});

    /** @class chlk.activities.lib.BodyClass*/
    ANNOTATION(
        [[String]],
        function BodyClass(clazz) {});


     /** @class chlk.activities.lib.PartialUpdateClass*/
    ANNOTATION(
        [[String]],
        function PartialUpdateClass(partialUpdateCls) {});

    /** @class chlk.activities.lib.ModelWaitClass*/
    ANNOTATION(
        [[String]],
        function ModelWaitClass(modelWaitCls) {});

    var PARTIAL_UPDATE_CLASS = 'partial-update';

    /** @class chlk.activities.lib.ChlkTemplateActivity*/

    CLASS(
        'ChlkTemplateActivity', EXTENDS(ria.mvc.TemplateActivity), [
            [[ria.reflection.ReflectionClass]],
            OVERRIDE, VOID, function processAnnotations_(ref) {
                BASE(ref);

               if (ref.isAnnotatedWith(chlk.activities.lib.PartialUpdateClass)){
                   this._partialUpdateCls = ref.findAnnotation(chlk.activities.lib.PartialUpdateClass).pop().partialUpdateCls;
               }else{
                   this._partialUpdateCls = PARTIAL_UPDATE_CLASS;
               }

                if (ref.isAnnotatedWith(chlk.activities.lib.ModelWaitClass)) {
                    this._modelWaitClass = ref.findAnnotation(chlk.activities.lib.ModelWaitClass).pop().modelWaitCls;
                }
            },
            OVERRIDE, VOID, function addPartialRefreshLoader(msg_) {

                var actualMsg = msg_ ? msg_ : "";
                var dontLoadMsg = chlk.activities.lib.DontShowLoader();

                if (actualMsg.indexOf(dontLoadMsg) != -1){
                    actualMsg = actualMsg.replace(dontLoadMsg, '');
                    msg_ = actualMsg;
                }
                else{
                    this.dom.addClass(this._partialUpdateCls);
                }
                BASE(msg_);
            },

            chlk.models.people.User, 'currentUser',

            chlk.models.common.Role, 'role',

            [[String]],
            OVERRIDE, VOID, function onModelComplete_(msg_) {
                this.dom.removeClass(this._partialUpdateCls);
                BASE(msg_);
            },

            [[ria.templates.Template, Object, String]],
            OVERRIDE, VOID, function onPrepareTemplate_(tpl, model, msg_) {
                BASE(tpl, model, msg_);
                tpl.options({
                    userRole: this.getRole(),
                    currentUser: this.getCurrentUser()
                })
            }

            //todo think about moving methods from children to base class
        ]);




    chlk.activities.lib.DontShowLoader = function() {
        return 'no-loading';
    }
});
