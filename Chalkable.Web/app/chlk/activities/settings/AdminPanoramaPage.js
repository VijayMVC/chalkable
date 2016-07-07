REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.AdminPanoramaTpl');
REQUIRE('chlk.templates.settings.AdminPanoramaCourseTypesTpl');
REQUIRE('chlk.templates.controls.PanoramaFilterBlockTpl');

NAMESPACE('chlk.activities.settings', function () {

    var timer, needToSubmit;

    /** @class chlk.activities.settings.AdminPanoramaPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.AdminPanoramaTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.settings.AdminPanoramaTpl, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.settings.AdminPanoramaCourseTypesTpl, 'course-types', '.course-types-cnt', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.controls.PanoramaFilterBlockTpl, 'filter-block', '.current-filters-block .filters-container', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.PartialUpdateRule(chlk.templates.controls.PanoramaFilterSelectsTpl, 'filter-selects', '.current-filter .additional-filters', ria.mvc.PartialUpdateRuleActions.Replace)],
        'AdminPanoramaPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',

            [ria.mvc.DomEventBind('click', '.submit-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function submitClick(node, event) {
                var filters = this.getFiltersObject_();
                this.dom.find('.filters-value').setValue(JSON.stringify(filters));
            },

            [ria.mvc.DomEventBind('click', '.add-subject-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function addSubjectClick(node, event) {
                var ids = [];
                this.dom.find('.course-type-block').forEach(function(node){
                    ids.push(node.getData('id'));
                });
                this.dom.find('.excluded-ids').setValue(JSON.stringify(ids));
            },

            function getStandardizedTestFilters_(node){
                var filters = [];
                node.find('.filter-block').forEach(function(block){
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

                return filters;
            },

            function getFiltersObject_(){
                var res = {}, courseTypeDefaultSettings = [], that = this;

                res.previousYearsCount = parseInt(this.dom.find('#previous-years').getValue(), 10);

                res.studentDefaultSettings = this.getStandardizedTestFilters_(this.dom.find('.student-filters-block'));

                this.dom.find('.course-type-block').forEach(function(node){
                    courseTypeDefaultSettings.push({
                        courseTypeId : node.getData('id'),
                        standardizedTestFilters: that.getStandardizedTestFilters_(node)
                    });
                });

                res.courseTypeDefaultSettings = courseTypeDefaultSettings

                return res;
            },

            [ria.mvc.DomEventBind('click', '.add-filter-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function addFilterClick(node, event) {
                var parent = node.parent('.filters-block');
                this.dom.find('.current-filters-block').removeClass('current-filters-block');
                parent.addClass('current-filters-block');
                var model = new chlk.models.controls.PanoramaFilterBlockViewData(this.getStandardizedTests());
                this.partialRefreshD(ria.async.Future.$fromData(model), 'filter-block');
            },

            [ria.mvc.DomEventBind('change', '.filter-type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function filterTypeSelect(node, event, selected) {
                var value = node.getValue();
                if (value){
                    value = new chlk.models.id.StandardizedTestId(parseInt(value,10));
                    var model = new chlk.models.controls.PanoramaFilterBlockViewData(this.getStandardizedTests(), value);
                    this.partialRefreshD(ria.async.Future.$fromData(model), 'filter-selects');
                }
            },

            [ria.mvc.DomEventBind('click', '.delete-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function deleteClick(node, event) {
                var form = node.parent('form'), block = node.parent('.filter-block');
                block.removeSelf();
            },

            [ria.mvc.DomEventBind('click', '.filter-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            function filterBlockClick(node, event) {
                var form = node.parent('form');
                form.find('.current-filter').removeClass('current-filter');
                node.addClass('current-filter');
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(msg_ == 'filter-block')
                    this.dom.find('current-filters-block').removeClass('current-filters-block');
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setStandardizedTests(model.getStandardizedTests());
            }
        ]);
});