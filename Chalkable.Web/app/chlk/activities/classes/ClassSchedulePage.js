REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassScheduleTpl');
REQUIRE('chlk.templates.calendar.announcement.DayPage');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassSchedulePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassScheduleTpl)],
        'ClassSchedulePage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._classId = null;
            },

            chlk.models.id.ClassId, function getClassId(){
                return this._classId;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.calendar.announcement.DayPage)],
            VOID, function updateCalendar(tpl, model, msg){
                tpl.options({
                    notMainCalendar: true,
                    controllerName: 'class',
                    actionName: 'scheduleUpdate',
                    params: [this.getClassId().valueOf()]
                });
                var container = this.dom.find('.calendar-section');
                container.empty();
                tpl.renderTo(container.removeClass('loading'));
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this._classId = model.getClazz().getId();
            }
        ]);
});