REQUIRE('chlk.models.id.PictureId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppPicture */
    CLASS(
        'AppPicture', [
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
