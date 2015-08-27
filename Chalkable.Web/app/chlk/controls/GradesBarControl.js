REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.GradeLevelService');

REQUIRE('chlk.controls.LeftRightToolbarControl');
REQUIRE('chlk.controls.ActionLinkControl');

NAMESPACE('chlk.controls', function () {

    /**
     * @class chlk.controls.GradesBarControl
     */
    var allItem;

    CLASS(
        'GradesBarControl', EXTENDS(chlk.controls.Base), [

            function $() {
                BASE();

                allItem = chlk.lib.serialize.ChlkJsonSerializer().deserialize({
                    name: 'All',
                    description: 'All',
                    id: ''
                }, chlk.models.grading.GradeLevel);
            },

            function prepareModel(model, includeAll) {
                var glsService = this.context.getService(chlk.services.GradeLevelService),
                    gls = model.getTopItems() || glsService.getClassesForTopBarSync(),
                    prependItems = includeAll ? [allItem] : [];

                return [].concat(prependItems, gls);
            },

            OVERRIDE, VOID, function onCreate_() {
                ASSET('~/assets/jade/controls/grade-bar.jade')(this);
            },

            function isPressed(item, currentGradeLevelsId){
                var id = item.getId() && item.getId().valueOf();
                var filteredItem = currentGradeLevelsId.filter(function(itemId){
                    return itemId == id;
                })[0];
                return !currentGradeLevelsId.length && !id || currentGradeLevelsId.length && filteredItem
            },

            function getCurrentIds(item, currentGradeLevelsId, isPressed_){
                var id = item.getId() && item.getId().valueOf();
                if(!id)
                    return '';
                var ids = currentGradeLevelsId.slice();
                if(isPressed_)
                    ids.splice(currentGradeLevelsId.indexOf(id.toString()), 1);
                else
                    ids.push(id);
                return ids.join(',');
            }
        ]);
});
