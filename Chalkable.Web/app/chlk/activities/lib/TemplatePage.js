REQUIRE('ria.mvc.TemplateActivity');

NAMESPACE('chlk.activities.lib', function () {

    /** @class chlk.activities.lib.PageClass */
    ANNOTATION(
        [[String]],
        function PageClass(clazz) {});

    var LOADING_CLASS = 'loading-page';

    /** @class chlk.activities.lib.TemplatePage*/
    CLASS(
        'TemplatePage', EXTENDS(ria.mvc.TemplateActivity), [
            function $() {
                BASE();
                this._wrapper = new ria.dom.Dom('#content-wrapper');
            },


            OVERRIDE, VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (ref.isAnnotatedWith(chlk.activities.lib.PageClass)){
                    this._pageClass = ref.findAnnotation(chlk.activities.lib.PageClass)[0].clazz;
                }else{
                    this._pageClass = null;
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                this._pageClass && this._wrapper.addClass(this._pageClass);
            },

            [[ria.async.Future]],
            OVERRIDE, ria.async.Future, function refreshD(future) {
                this.dom.addClass(LOADING_CLASS);
                return BASE(future);
            },

            [[String]],
            OVERRIDE, VOID, function onModelComplete_(msg_) {
                BASE();
                this.dom.removeClass(LOADING_CLASS);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this._pageClass && this._wrapper.removeClass(this._pageClass);
            }
        ]);
});
