REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassAttendance');
REQUIRE('chlk.templates.attendance.ClassList');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.ClassListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.ClassList)],
        'ClassListPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassAttendance)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                var container = this.dom.find('.container-' + model.getClassPersonId().valueOf());
                container.empty();
                tpl.renderTo(container);
            }
        ]);
});