REQUIRE('ria.mvc.DomActivity');
REQUIRE('ria.dom.Dom');
REQUIRE('ria.reflection.ReflectionFactory');

NAMESPACE('chlk.activities.lib', function () {

    /** @class chlk.activities.BindTemplate */
    ANNOTATION(function BindTemplate(template) {});

    /** @class chlk.activities.TemplateActivity*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        'TemplateActivity', EXTENDS(ria.mvc.DomActivity), [
            function $() {
                BASE();
                this.bind_();
            },

            OVERRIDE, ria.dom.Dom, function onDomCreate_() {
                var dom = new ria.dom.Dom();
                return dom.fromHTML('<div>Loading...</div>');
            },

            VOID, function bind_() {
                var ref = ria.reflection.ReflectionFactory(this.getClass());
                if (!ref.isAnnotatedWith(chlk.activities.lib.BindTemplate))
                    throw new ria.mvc.MvcException(ref.getName() + " should be annotated with " + ria.__API.getIdentifierOfType(chlk.activities.lib.BindTemplate));

                var binds = ref.findAnnotation(chlk.activities.lib.BindTemplate)[0].template;
                if (!Array.isArray(binds))
                    binds = [binds];

                var tpls = this.tpls = [];
                binds.forEach(function (tpl) {
                    if (tpl === undefined)
                        throw new ria.mvc.MvcException(ref.getName() + " is annotated with " + ria.__API.getIdentifierOfType(chlk.activities.lib.BindTemplate)
                            + ', but some templates classes appears to be not loaded: [' + binds.map(function (_) { return _ ? ria.__API.getIdentifierOfType(_) : _ }) + ']');

                    var tplRef = ria.reflection.ReflectionFactory(tpl);
                    var model = tplRef.findAnnotation(ria.templates.ModelBind)[0];
                    if (model == null)
                        throw new ria.mvc.MvcException(tplRef.getName() + " is not annotated with " +  ria.__API.getIdentifierOfType(ria.templates.ModelBind));

                    if (model.name_ == null)
                        throw new ria.mvc.MvcException(tplRef.getName() + " is annotated with " + ria.__API.getIdentifierOfType(ria.templates.ModelBind)
                            + ', but model class appears to be not loaded');

                    tpls.push({model:model.name_, tpl:tpl});
                });
            },
            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                var tpls = this.tpls.filter(function (_) {
                    if (ria.__API.isClassConstructor(_.model)) {
                        return model instanceof _.model;
                    }

                    if (ria.__API.isArrayOfDescriptor(_.model)) {
                        return Array.isArray(model) && model.every(function (__) { return __ instanceof _.model.clazz; })
                    }
                    return false;
                });

                if (tpls.length == 0)
                    throw new ria.mvc.MvcException(ria.__API.getIdentifierOfType(this.getClass())+ " has no template that is bind to model class "
                        + ria.__API.getIdentifierOfValue(model) + ", templates: " + this.tpls.map(function (_) { return ria.__API.getIdentifierOfType(_.tpl)+ '<' + ria.__API.getIdentifierOfType(_.model) + '>'}));

                if (tpls.length > 1)
                    throw new ria.mvc.MvcException(ria.__API.getIdentifierOfType(this.getClass())+ " has multiple templates that is bind to model class "
                        + ria.__API.getIdentifierOfValue(model) + ", templates: " + tpls.map(function (_) { return ria.__API.getIdentifierOfType(_.tpl)+ '<' + ria.__API.getIdentifierOfType(_.model)+ '>'}));

                var tplClass = tpls.pop().tpl;
                var tpl = new tplClass();
                tpl.assign(model);
                tpl.renderTo(this.dom.empty());
            },

            OVERRIDE, VOID, function onStop_() {
                this.dom.empty();
                BASE();
            },

            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
            }
        ]);
});
