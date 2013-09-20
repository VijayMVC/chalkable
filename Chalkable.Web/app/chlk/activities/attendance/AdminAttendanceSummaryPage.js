REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.AdminAttendanceSummaryTpl');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.AdminAttendanceSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('attendance')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.AdminAttendanceSummaryTpl)],
        'AdminAttendanceSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.attendance-top-box:not(.active)')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function boxClick(node, event) {
                    var index = node.getData('index');
                    this.dom.find('.attendance-top-box.active').removeClass('active');
                    this.dom.find('.students-statistic.active').removeClass('active');
                    this.dom.find('.students-statistic.page-' + index).addClass('active');
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
                    var hiddenBlock = this.dom.find('.hidden-students-block.no-used:eq(0)');
                    var height = hiddenBlock.find('.hidden-students-block-2').height();
                    hiddenBlock
                        .setCss('height', height)
                        .removeClass('no-used');
                    if(hiddenBlock.hasClass('last-block'))
                        node.parent('.more-container').hide();
            }
        ]);
});