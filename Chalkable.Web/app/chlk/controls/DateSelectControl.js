REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.DateSelectControl */
    CLASS(
        'DateSelectControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/dateSelect.jade')(this);
            },

            Array, 'days',

            Array, 'months',

            Array, 'years',

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                var value = attributes.value;
                delete attributes.name;
                if(value){
                    attributes.day = value.getDate();
                    attributes.month = value.getMonth() + 1;
                    attributes.year = value.getFullYear();
                }
                var monthData = [
                    [0,Msg.January],
                    [1,Msg.February],
                    [2,Msg.March],
                    [3,Msg.April],
                    [4,Msg.May],
                    [5,Msg.June],
                    [6,Msg.July] ,
                    [7,Msg.August],
                    [8,Msg.September],
                    [9,Msg.October],
                    [10,Msg.November],
                    [11,Msg.December]
                ];
                var daysData=[],days = 31,yearsData = [], currentYear = (getDate()).getFullYear();
                for(var i=1; i <= days; i++)daysData.push(i);
                for(i = currentYear; i >=  1900; --i){yearsData.push(i)};
                this.setMonths(monthData);
                this.setDays(daysData);
                this.setYears(yearsData);

                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var container = new ria.dom.Dom('#' + attributes.id);
                        var daySelect = container.find('.day-combo');
                        var monthSelect = container.find('.month-combo');
                        var yearSelect = container.find('.year-combo');
                        var hidden = container.find('.hidden-value');
                        jQuery('#' + attributes.id).find('select').on('change', function(event, options){
                            var dayValue = parseInt(daySelect.getValue(), 10);
                            var monthValue = parseInt(monthSelect.getValue(), 10);
                            var yearValue = yearSelect.getValue();
                            if(!new ria.dom.Dom(this).hasClass('day-combo')){
                                daySelect.setHTML('');
                                var days = daysInMonth((monthValue || 0) + 1, yearValue || 2000), wasSelected=false;
                                for(var i = 1; i <= days; i++){
                                    var option = new Option(i),selected=false;
                                    if(i === dayValue){
                                        selected=true;
                                        wasSelected = true;
                                    }
                                    if(i == days && !wasSelected){
                                        selected=true;
                                        dayValue=i;
                                    }

                                    jQuery(option).attr('selected', selected);
                                    daySelect.appendChild(option);
                                }
                                daySelect.trigger("liszt:updated");
                            }
                            if(dayValue && monthValue && yearValue)
                                hidden.setValue(monthValue + '/' + dayValue + '/' + yearValue);
                        });
                    }.bind(this));

                return attributes;
            }
        ]);
});