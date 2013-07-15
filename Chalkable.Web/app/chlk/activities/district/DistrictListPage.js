REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.district.Districts');

NAMESPACE('chlk.activities.district', function () {

    /** @class chlk.activities.districts.DistrictsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.district.Districts)],
        'DistrictListPage', EXTENDS(ria.mvc.TemplateActivity), [
            [[Object, String]],
            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);

                var tpl = new chlk.templates.district.Districts();
                tpl.assign(model);
                tpl.renderTo(this.dom.empty());
            }
        ]);
});