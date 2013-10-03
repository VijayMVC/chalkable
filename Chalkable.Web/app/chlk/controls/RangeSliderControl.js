REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.RangeSliderControl */




    /*
    * mixin RangeSlider(id, name)
     data = this.prepareData(attributes)
     label.slider-lb.start-slider-range=self.getStartRange()
     .new-slider(style="width: #{self.getSliderWidth()}px")
     .slider-scale
     each item, index in data.items
     .scale-el(style="left: #{self.getScaleLeftPosition(index, self.getSeparatorWidth()}px")
     span.scale-notches |
     span.scale-numbers item
     div(id=id)
     label.slider-lb.end-slider-range=self.getEndRange()

     *
    * */
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
                        node.find(".slider-lb.start-slider-range").text(that.getDisplayFieldValue(ui.values[0]));
                        node.find(".slider-lb.end-slider-range").text(that.getDisplayFieldValue(ui.values[1]));

                        that.onAfterSlide(event, ui);
                    },
                    stop : function (event, ui){
                        that.onStop(event, ui);
                        that.onAfterStop(event, ui);
                    }
                }).each(function() {
                        var opt = jQuery(this).data()['ui-slider'].options;

                        // Get the number of possible values
                        var vals = opt.max - opt.min;

                        // Position the labels
                        for (var i = 0; i <= vals; i++) {

                            // Create a new element and position it with percentages
                            var el = jQuery('<label>'+(i+opt.min)+'</label>').css('left',(i/vals*100)+'%');
                            // Add the element inside #slider
                            node.append(el);

                            var notch = jQuery('<label class="notch">|</label>').css('left',(i/vals*100)+'%');
                            node.append(notch);
                        }

                    });
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));
                return attributes;
            },

            function onStop(event, ui){
                var arr = [];
                var el = ui.values[0];
                while(el <= ui.values[1]){
                    arr.push(el);
                    el++;
                }
                this.setSliderItems(arr);
            },

            function onAfterStop(event, ui){},

            function onAfterSlide(event, ui){},

            function prepareData(start, end){

                this.setStartRange(start);
                this.setEndRange(end);

                var items = [];

                for (var i = start; i <= end; ++i){
                    items.push(i);
                }

                this.setSliderItems(items);
                //todo: set start and end label
            },

            function getDisplayFieldValue(value){
                return value;
                //todo: add th
            }

        ]);
});