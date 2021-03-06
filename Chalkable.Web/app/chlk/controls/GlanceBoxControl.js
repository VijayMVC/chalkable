REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.GlanceBoxControl */
    CLASS(
        'GlanceBoxControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/glance-box.jade')(this);
            },
            String, function getValueClass(value) {
                var res = '';
                if(isNaN(parseInt(value, 10)))
                    return '';
                if (value >= 100 && value < 1000) res = 'large';
                else if (value >= 1000) res = 'small';
                return res;
            },
            [[Object, String]],
            String, function getShortText(value1_, value2_){
                var res = value1_ + ' ' + value2_;
                /*if (value1 !== undefined && value1 !== null && value2 !== undefined && value2 !== null){
                    if(value1.length + value2.length > 9){
                        res = value1 + ' ' +  value2.slice(0, 8 - value1.length) + '...';
                    }
                }*/
                return res;
            },

            function getGlanceText(data){
                var value = data.value;
                var float = parseFloat(value);
                if(value && value.indexOf && value.indexOf(',') > -1)
                    float = parseFloat(value.replace(',', '.'));
                if(float || float === 0){
                    if(float == float.toFixed(2))
                        value = float;
                    else
                        value = float.toFixed(2);
                }
                return (value || value === 0) ? value : '';
            },

            [ria.mvc.DomEventBind('mouseout', '.glance-box.hover-action-box')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function glanceLeave(node, event) {
                var nodeT = new ria.dom.Dom(event.target);
                if(nodeT.hasClass('glance-box')){
                    node.find('.details-container').scrollTop(0);
                }
            },

            function prepareGlanceBox(){
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var textItems = activity.getDom().find('.glance-text:not(.processed)');
                        textItems.forEach(function(textItem){
                            var parentWidth = textItem.parent().width();
                            var width = textItem.valueOf()[0].scrollWidth;
                            if(width > parentWidth){
                                var currentFontSize = parseInt(textItem.getCss('font-size'), 10);
                                var newFontSize = Math.floor(currentFontSize * parentWidth / width);
                                textItem.setCss('font-size', newFontSize).addClass('processed');
                            }
                        });

                    }.bind(this));
            }

        ]);
});