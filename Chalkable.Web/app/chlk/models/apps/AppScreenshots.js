REQUIRE('chlk.models.apps.AppPicture');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppScreenshots*/
    CLASS(
        'AppScreenshots', [
            ArrayOf(chlk.models.apps.AppPicture), 'items',


            String, 'ids',
            Boolean, 'readOnly',


            [[ArrayOf(chlk.models.apps.AppPicture), Boolean]],
            function $(screenshots, isReadOnly){
                BASE();
                this.setItems(screenshots);

                var scrIds = screenshots.map(function(item){
                    return item.getPictureId().valueOf();
                }).join(",");

                this.setIds(scrIds);
                this.setReadOnly(isReadOnly);
            }

        ]);


});
