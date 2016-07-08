REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.BaseSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('attendance')],
        'BaseSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            Object, 'addedStudent',

            [ria.mvc.DomEventBind('change', '.student-search-input')],
                [[ria.dom.Dom, ria.dom.Event, Object]],
                VOID, function selectStudent(node, event, o) {
                    var form = node.parent().find('.show-student-form');
                    var student = o.selected;
                    form.find('[name=id]').setValue(student.getId().valueOf());
                    form.trigger('submit');
                    this.hidePopUp();
                    node.setValue('');
                    this.setAddedStudent(student);
            },

            [ria.mvc.DomEventBind('blur', '.student-search-input')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function hidePopUp(node_, event_) {
                    this.dom.find('.add-popup').fadeOut();
            },

            [ria.mvc.DomEventBind('click', '.top-number-box:not(.active-part)')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function boxClick(node, event) {
                    var index = node.getData('index');
                    this.dom.find('.active-part').removeClass('active-part');
                    this.dom.find('.page-' + index).addClass('active-part');
                    node.addClass('active-part');
            },

            [ria.mvc.DomEventBind('click', '.for-date-picker')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function forDatePickerClick(node, event) {
                    this.dom.find('#nowDateTime').trigger('focus');
            },

            [ria.mvc.DomEventBind('click', '.add-student-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function plusClick(node, event){
                node.find('.add-popup').fadeIn();
                setTimeout(function(){
                    node.find('input').trigger('focus');
                }, 1);
            },

            [ria.mvc.DomEventBind('click', '.all-button')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function allButtonClick(node, event) {
                var firstBlock = node.parent('.summary-students');
                var hiddenBlock = firstBlock.find('.hidden-students-block');
                var visibleBlock2 = firstBlock.find('.visible-block').find('.hidden-students-block-2');
                if(node.hasClass('less')){
                    hiddenBlock
                        .setCss('height', 0);
                    node.removeClass('less');
                    node.setHTML(Msg.All);
                    visibleBlock2.removeClass('expanded');
                }else{
                    var height = hiddenBlock.find('.hidden-students-block-2').height();
                    hiddenBlock
                        .setCss('height', height);
                    var interval, kil = 0;
                    interval = setInterval(function(){
                        var scrollToNode = firstBlock.next('.summary-students');
                        var dest = scrollToNode.exists() ? scrollToNode.offset().top - jQuery(window).height() : jQuery(document).height();
                        if(dest > jQuery(document).scrollTop())
                            jQuery("html, body").animate({scrollTop:dest}, 200);
                        kil++;
                        if(kil == 5)
                            clearInterval(interval);
                    }, 100);
                    node.addClass('less');
                    node.setHTML(Msg.Less);
                    visibleBlock2.addClass('expanded');
                }
            },

            [ria.mvc.DomEventBind('change', '#nowDateTime')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function selectedDayChange(node, event) {
                    node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('click')],
                [[ria.dom.Dom, ria.dom.Event]],
                VOID, function domClick(node, event) {
                    this.dom.find('.student.absolute').remove();
            }
        ]);
});