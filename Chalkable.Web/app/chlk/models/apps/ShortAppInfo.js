NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.ShortAppInfo*/
    CLASS(
        'ShortAppInfo', [
            String, 'name',
            String, 'url',
            [ria.serialize.SerializeProperty('videodemourl')],
            String, 'videoModeUrl',
            [ria.serialize.SerializeProperty('shortdescription')],
            String, 'shortDescription',
            String, 'description',
            [ria.serialize.SerializeProperty('smallpictureid')],
            chlk.models.id.PictureId, 'smallPictureId',
            [ria.serialize.SerializeProperty('bigpictureid')],
            chlk.models.id.PictureId, 'bigPictureId',


            [[String, String, String, String, String, chlk.models.id.PictureId, chlk.models.id.PictureId]],
            function $(name, url, videoModeUrl, shortDescr, descr, smallPictureId_, bigPictureId_){
                this.setName(name);
                this.setUrl(url);
                this.setVideoModeUrl(videoModeUrl);
                this.setShortDescription(shortDescr);
                this.setDescription(descr);
                if (smallPictureId_)
                    this.setSmallPictureId(smallPictureId_);
                if (bigPictureId_)
                    this.setBigPictureId(bigPictureId_);
            },

            Object, function getPostData(){
                return {
                    name: this.getName(),
                    url: this.getUrl(),
                    videomodeurl: this.getVideoModeUrl(),
                    shortdescription: this.getShortDescription(),
                    description: this.getDescription(),
                    smallpictureid: this.getSmallPictureId() ? this.getSmallPictureId().valueOf() : null,
                    bigpictureid: this.getBigPictureId() ? this.getBigPictureId().valueOf(): null
                }
            }
        ]);
});
