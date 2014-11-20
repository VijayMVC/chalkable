REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.PictureId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppPicture */
    CLASS(
        FINAL, UNSAFE, 'AppPicture',IMPLEMENTS(ria.serialize.IDeserializable),  [

            VOID, function deserialize(raw){
                this.pictureId = SJX.fromValue(raw.pictureid, chlk.models.id.PictureId);
                this.pictureUrl = SJX.fromValue(raw.pictureurl, String);
                this.templateDownloadLink = SJX.fromValue(raw.templatedownloadlink, String);
                this.pictureClass = SJX.fromValue(raw.pictureclass, String);
                this.width = SJX.fromValue(raw.width, Number);
                this.height = SJX.fromValue(raw.height, Number);
                this.title = SJX.fromValue(raw.title, String);
                this.editable = SJX.fromValue(raw.editable, Boolean);
            },
            chlk.models.id.PictureId, 'pictureId',
            String, 'pictureUrl',
            String, 'templateDownloadLink',
            String, 'pictureClass',
            Number, 'width',
            Number, 'height',
            String, 'title',
            Boolean, 'editable',

            [[chlk.models.id.PictureId, String, Number, Number, String, Boolean]],
            function $(pictureId_, pictureUrl_, width_, height_, title_, editable_){
                BASE();
                if (pictureId_)
                    this.setPictureId(pictureId_);
                if (pictureUrl_)
                    this.setPictureUrl(pictureUrl_);
                if (width_)
                    this.setWidth(width_);
                if (height_)
                    this.setHeight(height_);
                if (title_)
                    this.setTitle(title_);

                var cls = "small-icon-item";
                var tplImgType = 1;
                var tplDownloadLinkUrl = '/Developer/DownloadPictureTemplate?type=';

                if (this.getWidth() == 170){
                    cls = "big-icon-item";
                    tplImgType = 2;
                }

                if (this.getWidth() == 640){
                    cls = "screenshot-item";
                    tplImgType = 3;
                }
                this.setPictureClass(cls);
                this.setTemplateDownloadLink(tplDownloadLinkUrl + tplImgType);

                if (editable_)
                    this.setEditable(editable_);
            }
        ]);


});
