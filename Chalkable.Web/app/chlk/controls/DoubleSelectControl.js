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
                this.hideDropDown_(node);
            },

            [ria.mvc.DomEventBind('click', '.double-select .second-result')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function secondResultClick(node, event) {
                var dropDown = node.parent('.double-select ');
                dropDown.find('.selected-value').setValue(node.getData('id'));
                dropDown.find('.value-text').setText(node.getText());
                this.hideDropDown_(dropDown);
                dropDown.trigger('change');
            },

            function hideDropDown_(node){
                node.removeClass('chosen-container-active').removeClass('chosen-with-drop');
                node.parent('.double-select')
                    .find('.second-drop')
                    .addClass('x-hidden')
                    .find('.second-results')
                    .setHTML('');
            },

            [ria.mvc.DomEventBind('mouseover', '.double-select .first-result')],
            [[ria.dom.Dom, ria.dom.Event]],
            function firstResultOver(node, event){
                var items = node.getData('items'), res = '';

                items.forEach(function(item){
                    res += '<li class="active-result second-result" data-id="' + item.id  + '">' + item.name + '</li>'
                });

                node.parent('.double-select')
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
                        new ria.dom.Dom(document).on('click.doubleselect', function(node, event){
                            var target = new ria.dom.Dom(event.target);
                            if(!target.isOrInside('.double-select')){
                                this.hideDropDown_(ria.dom.Dom('.double-select'));
                            }
                        }.bind(this));
                    }.bind(this));
                    /*.onActivityClosed(function (activity, model) {
                        new ria.dom.Dom(document).off('click.doubleselect');
                    }*/
                return attributes;
            }
        ]);
});
