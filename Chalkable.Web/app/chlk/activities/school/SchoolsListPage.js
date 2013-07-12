REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.school.Schools');
REQUIRE('chlk.templates.school.SchoolsGrid');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.school.Schools)],
        'SchoolsListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [
            [[Object, String]],
            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                this.dom.find('.grid').removeClass('loading');
                var tpl = new chlk.templates.school.SchoolsGrid();
                tpl.assign(model);
                tpl.renderTo(this.dom.find('.grid').empty());
            },

            [[String]],
            OVERRIDE, VOID, function onModelWait_(msg_) {
                BASE(msg_);
                this.dom.find('.grid').addClass('loading');
            }
        ]);
});