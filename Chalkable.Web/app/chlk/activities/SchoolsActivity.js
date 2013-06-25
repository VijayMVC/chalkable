REQUIRE('ria.mvc.DomActivity');

REQUIRE('ria.dom.Dom');

REQUIRE('chlk.templates.Schools');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.Schools */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        'SchoolsActivity', EXTENDS(ria.mvc.DomActivity), [
            OVERRIDE, ria.dom.Dom, function onDomCreate_() {
                var dom = new ria.dom.Dom();
                return dom.fromHTML('<div>Loading...</div>');
            },

            OVERRIDE, VOID, function onRender_(data) {
                var tpl = new chlk.templates.Schools;
                tpl.assign(data);
                tpl.renderTo(this.dom.empty());
            }
        ]);
});