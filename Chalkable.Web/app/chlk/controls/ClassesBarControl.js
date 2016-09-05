/**
 * Created by Volodymyr on 12/29/2014.
 */

REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.controls.LeftRightToolbarControl');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.templates.classes.ClassPopupTpl');


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

                var classId = currentClassId && currentClassId.valueOf() ? currentClassId : null;
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASSES_BAR_ITEM_ID, classId);

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
                        var mainClassesBar = new ria.dom.Dom('.main-classes-bar');
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
                        });

                        classesBar.on('mouseover', '.group .class-button', function(node, event){

                            var itemId = Number.parseInt(node.getData('item-id'));
                            if(itemId){
                                var classId = new chlk.models.id.ClassId(itemId);
                                var c = this.context.getService(chlk.services.ClassService).getClassById(classId);
                                var popUpTpl = new chlk.templates.classes.ClassPopupTpl();
                                popUpTpl.assign(c);

                                var popupHolder = new ria.dom.Dom('#chlk-class-bar-popup-container');
                                var main = popupHolder.parent();
                                //var top =  node.offset().top + node.height() - main.offset().top + 90;
                                var left = node.offset().left - main.offset().left + 120;
                                popupHolder.setCss('left', left);
                                popupHolder.setCss('top', top);
                                popUpTpl.renderTo(popupHolder.setHTML(''));
                            }

                        }.bind(this));

                        classesBar.on('mouseleave  click', '.group .class-button', function(node,event){
                            var popupHolder = new ria.dom.Dom('#chlk-class-bar-popup-container');
                            popupHolder.setHTML('');
                        }.bind(this));

                        /*mainClassesBar.on('click', '.class-button', function(node, event){
                            var classId = JSON.parse(node.getData('item-id'));
                            this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASSES_BAR_ITEM_ID, classId ? new chlk.models.id.ClassId(classId.valueOf()) : null);
                        }.bind(this))*/

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
            //,

            //[ria.mvc.DomEventBind('mouseover', '.classes-bar .group .class-button')],
            //[[ria.dom.Dom, ria.dom.Event]],
            //function mouseOverClass(node, event){
            //
            //},
            //
            //[ria.mvc.DomEventBind('mouseleave', '.classes-bar .group .class-button')],
            //[[ria.dom.Dom, ria.dom.Event]],
            //function mouseLeaveClass(node, event){
            //    var popupHolder = new ria.dom.Dom('#chlk-class-bar-popup-container');
            //    popupHolder.setHTML('');
            //},
        ]);
});
