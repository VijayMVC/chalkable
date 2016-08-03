REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.TabEvents */
    ENUM('TabEvents', {
        TAB_CHANGED: 'tabchanged'
    });

    /** @class chlk.controls.TabsControl */
    CLASS(
        'TabsControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/tabs-control.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.tabs-block .tab-header:not(.active)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function activeTabClick(node, event) {
                var parent = node.parent('.tabs-block'), tab = node.getData('tab');
                parent.find('.active').removeClass('active');
                node.addClass('active');
                parent.find('.chart-content[data-tab=' + tab + ']').addClass('active');
                parent.trigger(chlk.controls.TabEvents.TAB_CHANGED.valueOf());
            }
        ]);
});