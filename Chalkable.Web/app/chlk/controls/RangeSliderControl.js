REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.RangeSliderControl */

    CLASS(
        'RangeSliderControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/range-slider.jade')(this);
            },

            ArrayOf(Number), 'sliderItems',

            Number, 'startRange',
            Number, 'endRange',
            Number, 'separatorWidth',
            String, 'name',

            [[Object]],
            VOID, function update(node){
                var that = this;
                var sliderItems = this.getSliderItems();

                var minVal = sliderItems[0];
                var maxVal = sliderItems[sliderItems.length - 1];

                node.slider({
                    range: true,
                    min: minVal,
                    max: maxVal,
                    values: [ minVal, maxVal],
                    animate: true,
                    slide: function(event, ui){
                        node.find(".start-range").text(that.getDisplayFieldValue(ui.values[0]));
                        node.find(".end-range").text(that.getDisplayFieldValue(ui.values[1]));
                    },
                    stop : function (event, ui){
                        that.onStop(node, event, ui);
                    }
                }).each(function() {
                    var opt = jQuery(this).data()['uiSlider'].options;
                    var vals = opt.max - opt.min;
                    for (var i = 0; i <= vals; i++) {
                        var el = jQuery('<label>'+(i+opt.min)+'</label>').css('left',(i/vals*100)+'%');
                        node.append(el);
                        var notch = jQuery('<label class="notch">|</label>').css('left',(i/vals*100)+'%');
                        node.append(notch);
                    }
                    var elLeft = jQuery('<div class="start-range">'+(that.getDisplayFieldValue(opt.min))+'</div>');
                    var elRight = jQuery('<div class="end-range">'+(that.getDisplayFieldValue(opt.max))+'</div>');

                    node.append(elLeft);
                    node.append(elRight);

                });
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.setName(attributes.name + 'SelectedValues');
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));
                return attributes;
            },

            function onStop(node, event, ui){
                var arr = [];
                var el = ui.values[0];
                while(el <= ui.values[1]){
                    arr.push(el);
                    el++;
                }
                this.setSliderItems(arr);
                jQuery('input[name=' + this.getName() + ']').val(arr.join(','));
                node.trigger('sliderChanged');
            },

            function prepareData(start, end){
                this.setStartRange(start);
                this.setEndRange(end);
                var items = [];
                for (var i = start; i <= end; ++i){
                    items.push(i);
                }
                this.setSliderItems(items);
            },

            function getDisplayFieldValue(value){
                if (value == 1) return "1st";
                if (value == 2) return "2nd";
                if (value == 3) return "3rd";
                return value + "th";
            }

        ]);
});