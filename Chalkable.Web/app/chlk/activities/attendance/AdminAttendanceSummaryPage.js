REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.AdminAttendanceSummaryTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceNowTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceDayTpl');
REQUIRE('chlk.templates.attendance.AdminAttendanceMpTpl');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.AdminAttendanceSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('attendance')],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceNowTpl, '', '.attendance-now', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceDayTpl, '', '.attendance-day', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.AdminAttendanceMpTpl, '', '.attendance-mp', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.attendance.AdminAttendanceSummaryTpl)],
        'AdminAttendanceSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.attendance-top-box:not(.active-part)')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function boxClick(node, event) {
                    var index = node.getData('index');
                    this.dom.find('.active-part').removeClass('active-part');
                    this.dom.find('.page-' + index).addClass('active-part');
                    node.addClass('active');
                    var left = node.getAttr('left');
                    var text = node.getAttr('text');
                    var slider = this.dom.find('.slider-container');
                    slider.setCss('left', left);
                    slider.find('.active').setHTML(text);
            },

            [ria.mvc.DomEventBind('click', '.more-students')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function moreStudentsClick(node, event) {
                    var hiddenBlock = node.parent('.attendance-students').find('.hidden-students-block.no-used:eq(0)');
                    var height = hiddenBlock.find('.hidden-students-block-2').height();
                    hiddenBlock
                        .setCss('height', height)
                        .removeClass('no-used');
                    if(hiddenBlock.hasClass('last-block'))
                        node.parent('.more-container').hide();
            },

            [ria.mvc.DomEventBind('change', '#nowDateTime')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function selectedDayChange(node, event) {
                    node.parent('form').trigger('submit');
            }
        ]);
});