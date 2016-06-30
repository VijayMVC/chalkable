/**
 * Created by Yurhan on 2/22/2016.
 */

REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.ApplicationContent*/
    CLASS(
        UNSAFE, FINAL,   'ApplicationContent',IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.text = SJX.fromValue(raw.Text, String);
                this.imageUrl = SJX.fromValue(raw.ImageUrl, String);
                this.contentId = SJX.fromValue(raw.ContentId, String);
                this.description = SJX.fromValue(raw.Description, String);
            },

            String, 'text',
            String, 'imageUrl',
            String, 'contentId',
            String, 'description',
        ]);
})