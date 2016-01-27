REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassAttendanceStatsTpl');
REQUIRE('chlk.templates.classes.ClassProfileAttendanceTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileAttendanceListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileAttendanceTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfileAttendanceTpl, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassList, '', '.attendances-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassAttendanceStatsTpl, null, '.attendances-chart' , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileAttendanceListPage', EXTENDS(chlk.activities.attendance.ClassListPage),[
            OVERRIDE, function getListModel(model){
                if(model instanceof chlk.models.attendance.ClassList)
                    return model;

                if(model instanceof chlk.models.classes.BaseClassProfileViewData)
                    return model.getClazz().getClassList();

                return null;
            }
        ]);
});