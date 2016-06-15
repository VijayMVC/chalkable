REQUIRE('chlk.controls.Base');
REQUIRE('chlk.templates.controls.PanoramaFilterBlockTpl');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.PanoramaFilterControl */
    CLASS(
        'PanoramaFilterControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/panorama-filter/filter.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.panorama-filter-form .add-filter-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function addFilterClick(node, event) {
                var form = node.parent('form');
                var tpl = new chlk.templates.controls.PanoramaFilterBlockTpl();
                var model = new chlk.models.controls.PanoramaFilterBlockViewData(this.getStandardizedTests());
                tpl.assign(model);
                var dom = new ria.dom.Dom().fromHTML(tpl.render());
                var target = form.find('.filters-container');
                dom.appendTo(target);
                this.context.getDefaultView().notifyControlRefreshed();
            },

            [ria.mvc.DomEventBind('change', '.panorama-filter-form .filter-type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function filterTypeSelect(node, event, selected) {
                var value = node.getValue();
                if (value){
                    value = new chlk.models.id.StandardizedTestId(parseInt(value,10));
                    var model = new chlk.models.controls.PanoramaFilterBlockViewData(this.getStandardizedTests(), value);
                    var tpl = new chlk.templates.controls.PanoramaFilterSelectsTpl();
                    tpl.assign(model);
                    var dom = new ria.dom.Dom().fromHTML(tpl.render()), form = node.parent('form');
                    var parent = form.find('.current-filter');
                    var target = parent.find('.additional-filters');
                    dom.appendTo(target.empty());
                    this.context.getDefaultView().notifyControlRefreshed();

                    this.formSubmitByNode_(node);
                }
            },

            [ria.mvc.DomEventBind('click', '.panorama-filter-form .delete-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function deleteClick(node, event) {
                var form = node.parent('form'), block = node.parent('.filter-block');
                block.removeSelf();
                this.formSubmitByNode_(null, form);
            },

            [ria.mvc.DomEventBind('click', '.panorama-filter-form .filter-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            function filterBlockClick(node, event) {
                var form = node.parent('form');
                form.find('.current-filter').removeClass('current-filter');
                node.addClass('current-filter');
            },

            [ria.mvc.DomEventBind('change', '.panorama-filter-ctn .submit-after-change')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function submitItemChange(node, event, selected) {
                this.formSubmitByNode_(node);
            },

            function formSubmitByNode_(node, form_) {
                var filters = [], form = form_ || node.parent('form');
                form.removeClass('working');

                var res = this.getFiltersObject_(form);

                this.checkRestoreBtn_(form, res);

                form.find('.filters-value').setValue(JSON.stringify(res));
                form.trigger('submit');
            },

            function checkRestoreBtn_(form, currentFilters){
                var oldFilters = form.getData('filters'),
                    oldYears = oldFilters.schoolYearIds.sort().join(','),
                    currentYears = currentFilters.schoolYearIds.sort().join(','),
                    btn = form.find('.restore-btn');

                var oldMap = oldFilters.standardizedTestFilters.map(function(item){
                    return [item.standardizedTestId, item.componentId, item.scoreTypeId].join(',')
                }).join(',');

                var currentMap = currentFilters.standardizedTestFilters.map(function(item){
                    return [item.standardizedTestId, item.componentId, item.scoreTypeId].join(',')
                }).join(',');

                if(oldYears == currentYears && oldMap == currentMap)
                    btn.setAttr('disabled', 'disabled');
                else
                    btn.removeAttr('disabled');
            },

            function getFiltersObject_(form){
                var filters = [];
                form.find('.filter-block').forEach(function(block){
                    var typeSelect = block.find('.filter-type-select'), type = typeSelect.getValue();
                    if(type){
                        var o = {
                            standardizedTestId : parseInt(type, 10),
                            componentId : parseInt(block.find('.component-select').getValue(), 10),
                            scoreTypeId : parseInt(block.find('.score-type-select').getValue(), 10)
                        };
                        filters.push(o);
                    }
                });

                var res = {
                    standardizedTestFilters: filters,
                    schoolYearIds: form.find('.school-years-select').getValue()
                };

                return res;
            },

            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',

            [[Object]],
            Object, function prepare(attributes, standardizedTests) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.setStandardizedTests(standardizedTests.filter(function(item){
                    return item.getComponents().length && item.getScoreTypes().length
                }));
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var form = ria.dom.Dom('#'+attributes.id).parent('form'),
                            filters = this.getFiltersObject_(form);

                        form.addClass('panorama-filter-form')
                            .setData('filters', filters);
                    }.bind(this));
                return attributes;
            }
        ]);
});