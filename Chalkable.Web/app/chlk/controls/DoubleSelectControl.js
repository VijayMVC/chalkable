REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.DoubleSelectEvents */
    /*ENUM('DoubleSelectEvents', {
        HIDE_DROP_DOWN: 'hidedropdown'
    });*/

    /** @class chlk.controls.DoubleSelectControl */
    CLASS(
        'DoubleSelectControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/double-select.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.double-select:not(.chosen-container-active)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function doubleSelectClick(node, event) {
                node.addClass('chosen-container-active').addClass('chosen-with-drop');
            },

            /*[ria.mvc.DomEventBind(chlk.controls.DoubleSelectEvents.HIDE_DROP_DOWN.valueOf(), '.double-select')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function hideDropDownEvent(node, event) {
                this.hideDropDown_(node);
            },*/

            [ria.mvc.DomEventBind('click', '.double-select.chosen-container-active')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function activeDoubleSelectClick(node, event) {
                node.is('.chosen-container-active') && this.hideDropDown_(node);
            },

            [ria.mvc.DomEventBind('click', '.double-select .second-result')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function secondResultClick(node, event) {
                var dropDown = node.parent('.double-select '),
                    yearItem = dropDown.find('.first-result.active-item'),
                    startDate = yearItem.getData('start-date') || '',
                    endDate = yearItem.getData('end-date') || '',
                    yearName = yearItem.getHTML();
                dropDown.find('.selected-value').setValue(node.getData('id'));
                dropDown.find('.selected-year-name').setValue(yearName);
                dropDown.find('.selected-year-start-date').setValue(startDate);
                dropDown.find('.selected-year-end-date').setValue(endDate);
                dropDown.find('.selected-class-name').setValue(node.getHTML());
                dropDown.find('.value-text').setText(node.getText());
                dropDown.find('.second-result.selected-item').removeClass('selected-item');
                dropDown.find('.first-result.selected-item').removeClass('selected-item');
                dropDown.find('.first-result.active-item').addClass('selected-item');
                node.addClass('selected-item');
                this.hideDropDown_(dropDown, true);
                dropDown.trigger('change');
            },

            function hideDropDown_(node, selected_){
                node.removeClass('chosen-container-active').removeClass('chosen-with-drop');
                node.parent('.double-select')
                    .find('.second-drop')
                    .addClass('x-hidden')
                    .find('.second-results')
                    .setHTML('');

                node.find('.active-item').removeClass('active-item');
            },

            [ria.mvc.DomEventBind('mouseover', '.double-select .first-result')],
            [[ria.dom.Dom, ria.dom.Event]],
            function firstResultOver(node, event){
                var items = node.getData('items'), res = '', dropDown = node.parent('.double-select');
                var value = dropDown.find('.selected-value').getValue();

                items.forEach(function(item){
                    res += '<li class="active-result second-result ' + (value == item.id ? 'selected-item' : '') + '" data-id="' + item.id  + '" data-tooltip-type="overflow" data-tooltip="' + item.name + '">' + item.name + '</li>'
                });

                dropDown.find('.first-result.active-item').removeClass('active-item');
                node.addClass('active-item');

                dropDown
                    .find('.second-drop')
                    .removeClass('x-hidden')
                    .find('.second-results')
                    .setHTML(res);
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var doc = ria.dom.Dom(document);
                        new doc.off('click.doubleselect');
                        new doc.on('click.doubleselect', function(node, event){
                            var target = new ria.dom.Dom(event.target);
                            if(!target.isOrInside('.double-select')){
                                this.hideDropDown_(ria.dom.Dom('.double-select.chosen-container-active'));
                            }
                        }.bind(this));
                    }.bind(this));
                    /*.onActivityClosed(function (activity, model) {
                        new ria.dom.Dom(document).off('click.doubleselect');
                    }*/
                return attributes;
            },

            function prepareItems(items){
                var res = []
                items.forEach(function(item){
                    var classes = item.getClasses();
                    if(classes.length){
                        var sY = item.getSchoolYear();
                        res.push({
                            name: item.getSchoolName() + ' - ' + sY.getName(),
                            id: sY.getId(),
                            startDate: sY.getStartDate() && sY.getStartDate().toStandardFormat(),
                            endDate: sY.getEndDate() && sY.getEndDate().toStandardFormat(),
                            values: classes.map(function(clazz){
                                return {
                                    name: clazz.getFullClassName(),
                                    id: clazz.getId().valueOf()
                                }
                            })
                        })
                    }
                });
                return res;
            }
        ]);
});
