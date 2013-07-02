REQUIRE('chlk.activities.TemplateActivity');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.PageClass */
    ANNOTATION(
        [[String]],
        function PageClass(clazz) {});

    /** @class chlk.activities.TemplatePage*/
    CLASS(
        'TemplatePage', EXTENDS(chlk.activities.TemplateActivity), [
            function $() {
                BASE();
                this._wrapper = new ria.dom.Dom('#content-wrapper');
            },

            OVERRIDE, VOID, function bind_() {
                BASE();
                var ref = ria.reflection.ReflectionFactory(this.getClass());
                if (ref.isAnnotatedWith(chlk.activities.PageClass)){
                    this._pageClass = ref.findAnnotation(chlk.activities.PageClass)[0].clazz;
                }else{
                    this._pageClass = null;
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                this._pageClass && this._wrapper.addClass(this._pageClass);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this._pageClass && this._wrapper.removeClass(this._pageClass);
            }
        ]);
});
