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
    var baseMargin = 0;

    CLASS(
        'ClassesBarControl', EXTENDS(chlk.controls.Base), [
            function prepareModel(classes) {
                var mpService = this.context.getService(chlk.services.MarkingPeriodService),
                    clsService = this.context.getService(chlk.services.ClassService),
                    cls = classes.getTopItems();

                var data = mpService.getMarkingPeriodsSync().map(function (mp) {
                    return {
                        id: mp.getId(),
                        title: mp.getName(),
                        items: cls.filter(function (c) {
                            var id = c.getId();
                            if (id.valueOf() == '')
                                return true;

                            return clsService.getMarkingPeriodRefsOfClass(id).indexOf(mp.getId()) >= 0;
                        })
                    }
                });

                data.currentMPId = mpService.getCurrentMarkingPeriod().getId();

                return data;
            },

            OVERRIDE, VOID, function onCreate_() {
                ASSET('~/assets/jade/controls/class-bar.jade')(this);

                var self = this;

                jQuery(window).on('resize', function(){
                    setTimeout(self.updateClassesBar, 1);
                });
            },

            function processAttrs(attributes) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        baseMargin = parseInt(new ria.dom.Dom('.classes-bar').find('.group').find('>a').getCss('margin-right'), 10);
                        this.updateClassesBar();
                    }.bind(this));
            },

            function updateClassesBar(){
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
            }
        ]);
});
