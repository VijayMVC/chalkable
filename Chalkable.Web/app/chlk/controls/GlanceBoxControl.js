REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.GlanceBoxControl */
    CLASS(
        'GlanceBoxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/glance-box.jade')(this);
            },
            [[Number]],
            String, function getValueClass(value) {
                var res = '';
                if (value >= 100 && value < 1000) res = 'large';
                else if (value >= 1000) res = 'small';
                return res;
            },
            [[Object, String]],
            String, function getShortText(value1, value2){
                var res = value1 + ' ' + value2;
                /*if (value1 !== undefined && value1 !== null && value2 !== undefined && value2 !== null){
                    if(value1.length + value2.length > 9){
                        res = value1 + ' ' +  value2.slice(0, 8 - value1.length) + '...';
                    }
                }*/
                return res;
            },

            [ria.mvc.DomEventBind('mouseout', '.glance-box.hover-action-box')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function glanceLeave(node, event) {
                var nodeT = new ria.dom.Dom(event.target);
                if(nodeT.hasClass('glance-box')){
                    node.find('.details-container').scrollTop(0);
                }
            }

        ]);
});