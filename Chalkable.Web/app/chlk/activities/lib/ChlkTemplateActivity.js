REQUIRE('ria.mvc.TemplateActivity');
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

    var PARTIAL_UPDATE_CLASS = 'partial-update';

    /** @class chlk.activities.lib.ChlkTemplateActivity*/




    CLASS(
        'ChlkTemplateActivity', EXTENDS(ria.mvc.TemplateActivity), [
            [[String]],
            OVERRIDE, VOID, function addPartialRefreshLoader(msg_) {

                var actualMsg = msg_ ? msg_ : "";
                var dontLoadMsg = chlk.activities.lib.DontShowLoader();

                if (actualMsg.indexOf(dontLoadMsg) != -1){
                    actualMsg = actualMsg.replace(dontLoadMsg, '');
                    msg_ = actualMsg;
                }
                else{
                    this.dom.addClass(PARTIAL_UPDATE_CLASS);
                }
                BASE(msg_);
            },

            chlk.models.common.Role, 'role',

            [[String]],
            OVERRIDE, VOID, function onModelComplete_(msg_) {
                this.dom.removeClass(PARTIAL_UPDATE_CLASS);
                BASE(msg_);
            },

            [[ria.templates.Template, Object, String]],
            OVERRIDE, VOID, function onPrepareTemplate_(tpl, model, msg_) {
                BASE(tpl, model, msg_);
                tpl.options({
                    userRole: this.getRole()
                })
            }

            //todo think about moving methods from children to base class
        ]);




    chlk.activities.lib.DontShowLoader = function() {
        return 'no-loading';
    }
});
