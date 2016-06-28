REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class app.controls.Grid */
    CLASS(
        'GridControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/grid.jade')(this);
            }
        ]);
});