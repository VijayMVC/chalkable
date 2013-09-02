REQUIRE('ria.mvc.TemplateActivity');

NAMESPACE('chlk.activities.lib', function () {

    /** @class chlk.activities.lib.PageClass */
    ANNOTATION(
        [[String]],
        function PageClass(clazz) {});

    /** @class chlk.activities.lib.BodyClass*/
    ANNOTATION(
        [[String]],
        function BodyClass(clazz) {});

    /** @class chlk.activities.lib.TemplatePage*/

    var LOADING_CLASS = 'loading-page';

    CLASS(
        'TemplatePage', EXTENDS(ria.mvc.TemplateActivity), [
            function $() {
                BASE();
                this._wrapper = new ria.dom.Dom('#content-wrapper');
                this._body = new ria.dom.Dom('body');
            },


            OVERRIDE, VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (ref.isAnnotatedWith(chlk.activities.lib.PageClass)){
                    this._pageClass = ref.findAnnotation(chlk.activities.lib.PageClass)[0].clazz;
                }else{
                    this._pageClass = null;
                }

                if (ref.isAnnotatedWith(chlk.activities.lib.BodyClass)){
                    this._bodyClass = ref.findAnnotation(chlk.activities.lib.BodyClass)[0].clazz;
                }else{
                    this._bodyClass = null;
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                this._pageClass && this._wrapper.addClass(this._pageClass);
                this._bodyClass && this._body.addClass(this._bodyClass);
            },

            OVERRIDE, VOID, function startFullLoading() {
                BASE();
                this.dom.addClass(LOADING_CLASS);
            },

            OVERRIDE, VOID, function stopLoading() {
                BASE();
                this.dom.removeClass(LOADING_CLASS);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this._pageClass && this._wrapper.removeClass(this._pageClass);
                this._bodyClass && this._body.removeClass(this._bodyClass);
            }
        ]);
});
