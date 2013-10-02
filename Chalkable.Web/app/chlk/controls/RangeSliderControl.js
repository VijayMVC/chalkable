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

            function getScaleLeftPosition(index, sWidth){
                return parseInt(index* sWidth);
            },

            function getSliderWidth(){
                return 150;
            },

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
                    values: [ that.getStartRange(), that.getEndRange()],
                    animate: true,
                    slide: function(event, ui){
                        that.onSlide(event, ui);
                        that.onAfterSlide(event, ui);
                    },
                    stop : function (event, ui){
                        that.onStop(event, ui);
                        that.onAfterStop(event, ui);
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

            function onSlide(event, ui){
                var node = jQuery(this.el.dom);
                node.find(".slider-lb.start-slider-range").text(this.getDisplayFieldValue(ui.values[0]));
                node.find(".slider-lb.end-slider-range").text(this.getDisplayFieldValue(ui.values[1]));
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

                var itemsLength = items.length;
                if (itemsLength == 0)
                    itemsLength = 1;

                this.setSeparatorWidth(this.getSliderWidth() / itemsLength);
            },

            function getDisplayFieldValue(value){
                return value;
                //todo: add th
            }

        ]);
});