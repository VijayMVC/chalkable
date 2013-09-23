REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.models.calendar.announcement.Month');
REQUIRE('chlk.templates.calendar.announcement.MonthCalendarBodyTpl');
REQUIRE('chlk.templates.classes.TopBar');
REQUIRE('chlk.templates.grading.GradeLevelForTopBar');

NAMESPACE('chlk.templates.calendar.announcement', function () {
    "use strict";

    /** @class chlk.templates.calendar.announcement.MonthPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Month)],
        [chlk.activities.lib.PageClass('calendar')],
        'MonthPage', EXTENDS(chlk.templates.calendar.BaseCalendarTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsForToolBar',

            Object, function getDataForToolBar(){
                var model = this.getModel();
                var res = {};
                res.tpl = chlk.templates.classes.TopBar;
                res.data = model.getTopData().getTopItems();
                res.selectedItemId = model.getTopData().getSelectedItemId();
                res.multiple = false;
                if(model.isAdmin()){
                    res.tpl = chlk.templates.grading.GradeLevelForTopBar;
                    res.data = model.getGradeLevelsForToolBar().getTopItems();
                    res.selectedItemId = null;
                    res.multiple = true;
                    res.selectedIds = model.getGradeLevelsForToolBar().getSelectedIds();
                }
                return res;
            }
        ]);
});