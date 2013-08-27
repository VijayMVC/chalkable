REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppGeneralInfoViewData*/
    CLASS(
        'AppGeneralInfoViewData', [
            chlk.models.apps.Application, 'app',
            String, 'appThumbnail',
            [[chlk.models.apps.Application]],
            function $(app){
                this.setApp(app);



                //var pictureId = app.getSmallPictureId().valueOf();
                //this.setAppThumbnal(pictureId ? )
                //if (){
                //    appThumbnal =
                //}
            }
        ]);
});
