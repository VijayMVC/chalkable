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
    CLASS(
        'ClassesBarControl', EXTENDS(chlk.controls.Base), [
            function prepareModel(classes) {
                var mpService = this.context.getService(chlk.services.MarkingPeriodService),
                    clsService = this.context.getService(chlk.services.ClassService),
                    cls = classes.getTopItems();

                return mpService.getMarkingPeriodsSync().map(function (mp) {
                    return {
                        title: mp.getName(),
                        items: cls.filter(function (c) {
                            var id = c.getId();
                            if (id.valueOf() == '')
                                return true;

                            return clsService.getMarkingPeriodRefsOfClass(id).indexOf(mp.getId()) >= 0;
                        })
                    }
                });
            },

            OVERRIDE, VOID, function onCreate_() {
                ASSET('~/assets/jade/controls/class-bar.jade')(this);
            }
        ]);
});
