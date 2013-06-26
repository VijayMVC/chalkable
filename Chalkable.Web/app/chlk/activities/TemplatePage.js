REQUIRE('chlk.activities.TemplateActivity');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.PageClass */
    ANNOTATION(
        [[String]],
        function PageClass(clazz) {});

    var pageClass, wrapper = new ria.dom.Dom('#content-wrapper');

    /** @class chlk.activities.TemplatePage*/
    CLASS(
        'TemplatePage', EXTENDS(chlk.activities.TemplateActivity), [
            function $() {
                BASE();
            },

            OVERRIDE, VOID, function bind_() {
                BASE();
                var ref = ria.reflection.ReflectionFactory(this.getClass());
                if (ref.isAnnotatedWith(chlk.activities.PageClass)){
                    pageClass = ref.findAnnotation(chlk.activities.PageClass)[0].clazz;
                }else{
                    pageClass = null
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                pageClass && wrapper.addClass(pageClass);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                pageClass && wrapper.removeClass(pageClass);
            }
        ]);
});
