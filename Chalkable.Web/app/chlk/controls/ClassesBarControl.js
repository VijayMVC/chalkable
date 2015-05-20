/**
 * Created by Volodymyr on 12/29/2014.
 */

REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.controls.LeftRightToolbarControl');
REQUIRE('chlk.controls.ActionLinkControl');

NAMESPACE('chlk.controls', function () {

    /**
     * @class chlk.controls.ClassesBarControl
     */
    var baseMargin = 0,
        allItem;

    CLASS(
        'ClassesBarControl', EXTENDS(chlk.controls.Base), [

            function $() {
                BASE();

                allItem = chlk.lib.serialize.ChlkJsonSerializer().deserialize({
                    name: 'All',
                    description: 'All',
                    id: ''
                }, chlk.models.classes.Class);
            },

            function prepareModel(model, includeAll) {
                var mpService = this.context.getService(chlk.services.MarkingPeriodService),
                    clsService = this.context.getService(chlk.services.ClassService),
                    cls = model.getTopItems() || clsService.getClassesForTopBarSync(),
                    prependItems = includeAll ? [allItem] : [];

                var currentMPId = mpService.getCurrentMarkingPeriod().getId();
                var data = mpService.getMarkingPeriodsSync()
                    .map(function (mp) {
                        return {
                            id: mp.getId(),
                            title: mp.getName(),
                            items: [].concat(prependItems, cls.filter(function (c) {
                                var id = c.getId();
                                if (id.valueOf() == '')
                                    return true;

                                return clsService.getMarkingPeriodRefsOfClass(id).indexOf(mp.getId()) >= 0;
                            }))
                        }
                    });


                var currentClassId = model.getSelectedItemId();
                if (currentClassId && currentClassId.valueOf()) {
                    var currentClassMPs = clsService.getMarkingPeriodRefsOfClass(currentClassId);
                    if (currentClassMPs.indexOf(currentMPId) < 0) {
                        currentMPId = currentClassMPs
                            .map(function (x) { return [x, Math.abs(x - currentMPId)]; })
                            .sort(function (_1, _2) { return _1[1] - _2[1]; })
                            [0][0];
                    }
                }

                data.currentMPId = currentMPId;

                return data;
            },

            OVERRIDE, VOID, function onCreate_() {
                ASSET('~/assets/jade/controls/class-bar.jade')(this);

                var self = this;

                jQuery(window).on('resize', function(){
                    setTimeout(function(){
                        self.updateClassesBar(true);
                    }, 1);
                });
            },

            function processAttrs(attributes) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var classesBar = new ria.dom.Dom('.classes-bar');
                        baseMargin = parseInt(classesBar.find('.group').find('>a').getCss('margin-right'), 10);
                        this.updateClassesBar();
                        var selectedGroupId = attributes.selectedGroupId || classesBar.find('.group:first-child').getData('id');
                        var group = classesBar.find('.group[data-id=' + attributes.selectedGroupId + ']');

                        if(!group.is(':first-child'))
                            classesBar.find('.second-container').setCss('left', classesBar.offset().left - group.offset().left);

                        classesBar.find('.group-title').setHTML(group.getData('name'));
                        classesBar.setData('group-id', group.getData('id'));
                        classesBar.on(chlk.controls.LRToolbarEvents.AFTER_ANIMATION.valueOf(), function(node, event){
                            var left = parseInt(node.find('.second-container').getCss('left'), 10);
                            var offsetLeft = node.offset().left, eps = 10;
                            node.find('.group').forEach(function(group){
                                var groupOffsetLeft = group.offset().left - offsetLeft;
                                if(groupOffsetLeft < eps && groupOffsetLeft > -group.width() + eps){
                                    classesBar.find('.group-title').setHTML(group.getData('name'));
                                    classesBar.setData('group-id', group.getData('id'));
                                }
                            })
                        })

                    }.bind(this));
            },

            function updateClassesBar(resize_){
                var classesBar = new ria.dom.Dom('.classes-bar');
                var width = classesBar.width();
                classesBar.find('.group').forEach(function(node){
                    node.setCss('width', 'auto');
                    var contentWidth = node.width();
                    var resMultiplier = Math.ceil(contentWidth / width);
                    node.setCss('width', resMultiplier * width);
                    var elements = node.find('>a');
                    var elWidthWithoutBorders = elements.width();
                    var elWidth = elWidthWithoutBorders + parseInt(elements.getCss('border-right-width'), 10) + parseInt(elements.getCss('border-left-width'), 10);
                    var baseAllWidth = elWidth + baseMargin;
                    var allWidth = width + baseMargin;
                    var count = Math.floor((allWidth) / baseAllWidth);
                    var additionalMargin = Math.floor((allWidth - (count * baseAllWidth)) / count);
                    if(additionalMargin > 0)
                        node.find('>a:not(:last-child)').setCss('margin-right', baseMargin + additionalMargin);
                });
                if(resize_){
                    var groupId = classesBar.getData('group-id');
                    var firstGroup = classesBar.find('.group:first-child');
                    if(firstGroup.getData('id') != groupId){
                        var secondContainer = classesBar.find('.second-container');
                        var curGroup = classesBar.find('.group[data-id=' + groupId + ']');
                        secondContainer.addClass('freezed')
                            .setCss('left', firstGroup.offset().left - curGroup.offset().left);
                        setTimeout(function(){
                            secondContainer.removeClass('freezed');
                        }, 1)
                    }
                }

            }
        ]);
});
