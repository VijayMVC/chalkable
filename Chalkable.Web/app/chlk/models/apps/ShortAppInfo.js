NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.ShortAppInfo*/
    CLASS(
        'ShortAppInfo', [
            String, 'name',
            String, 'url',
            [ria.serialize.SerializeProperty('videodemourl')],
            String, 'videoDemoUrl',
            [ria.serialize.SerializeProperty('shortdescription')],
            String, 'shortDescription',
            String, 'description',
            Boolean, 'advancedApp',
            [ria.serialize.SerializeProperty('smallpictureid')],
            chlk.models.id.PictureId, 'smallPictureId',
            [ria.serialize.SerializeProperty('bigpictureid')],
            chlk.models.id.PictureId, 'bigPictureId',

            [ria.serialize.SerializeProperty('externalattachpictureid')],
            chlk.models.id.PictureId, 'externalAttachPictureId',

            [[String, String, String, String, String, Boolean, chlk.models.id.PictureId, chlk.models.id.PictureId, chlk.models.id.PictureId]],
            function $(name, url, videoDemoUrl, shortDescr, descr, advancedApp, smallPictureId_, bigPictureId_, externalAttachPictureId_){
                BASE();
                this.setName(name);
                this.setUrl(url);
                this.setVideoDemoUrl(videoDemoUrl);
                this.setShortDescription(shortDescr);
                this.setDescription(descr);
                if (smallPictureId_)
                    this.setSmallPictureId(smallPictureId_);
                if (bigPictureId_)
                    this.setBigPictureId(bigPictureId_);
                if(externalAttachPictureId_)
                    this.setExternalAttachPictureId(externalAttachPictureId_);
                this.setAdvancedApp(advancedApp);
            },

            Object, function getPostData(){
                return {
                    name: this.getName(),
                    url: this.getUrl(),
                    videodemourl: this.getVideoDemoUrl(),
                    shortdescription: this.getShortDescription(),
                    description: this.getDescription(),
                    smallpictureid: this.getSmallPictureId() ? this.getSmallPictureId().valueOf() : null,
                    bigpictureid: this.getBigPictureId() ? this.getBigPictureId().valueOf(): null,
                    externalattachpictureid: this.getExternalAttachPictureId() ? this.getExternalAttachPictureId().valueOf() : null,
                    advancedapp: this.isAdvancedApp()
                }
            }
        ]);
});
