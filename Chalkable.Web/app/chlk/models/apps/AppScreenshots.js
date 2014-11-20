REQUIRE('chlk.models.apps.AppPicture');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppScreenshots*/
    CLASS(
        UNSAFE, FINAL, 'AppScreenshots',IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw){
                this.ids = SJX.fromValue(raw.ids, String);
                this.items = SJX.fromArrayOfDeserializables(raw.items, chlk.models.apps.AppPicture);
                this.templateDownloadLink = SJX.fromValue(raw.templatedownloadlink, String);
                this.readOnly = SJX.fromValue(raw.readonly, Boolean);
            },
            ArrayOf(chlk.models.apps.AppPicture), 'items',
            String, 'ids',
            Boolean, 'readOnly',
            String, 'templateDownloadLink',

            [[ArrayOf(chlk.models.apps.AppPicture), Boolean]],
            function $(screenshots, isReadOnly){
                BASE();
                this.setItems(screenshots);

                var scrIds = screenshots.map(function(item){
                    return item.getPictureId().valueOf();
                }).join(",");

                this.setIds(scrIds);
                this.setTemplateDownloadLink('/Developer/DownloadPictureTemplate?type=3');
                this.setReadOnly(isReadOnly);
            }

        ]);


});
