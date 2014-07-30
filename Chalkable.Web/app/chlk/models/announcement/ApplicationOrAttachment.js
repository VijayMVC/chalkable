REQUIRE('chlk.models.apps.BannedAppData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ApplicationOrAttachmentEnum*/
    ENUM('ApplicationOrAttachmentEnum', {
        DOCUMENT: 0,
        PICTURE: 1,
        OTHER: 2,
        APPLICATION: 3
    });

    /** @class chlk.models.announcement.ApplicationOrAttachment*/
    CLASS(
        'ApplicationOrAttachment', [
            Object, 'id',

            Boolean, 'owner',

            Number, 'order',

            chlk.models.announcement.ApplicationOrAttachmentEnum, 'type',

            String, 'name',

            String, 'pictureHref',

            String, 'url',

            String, 'editUrl',

            String, 'gradingViewUrl',

            String, 'viewUrl',

            chlk.models.apps.BannedAppData, 'banInfo',

            function $(id_, owner_, order_, type_, name_, pictureUrl_, url_, editUrl_, gradingViewUrl_, viewUrl_, banInfo_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(owner_)
                    this.setOwner(owner_);
                if(order_)
                    this.setOrder(order_);
                if(type_)
                    this.setType(type_);
                if(name_)
                    this.setName(name_);
                if(pictureUrl_)
                    this.setPictureHref(pictureUrl_);
                if(url_)
                    this.setUrl(url_);
                if(editUrl_)
                    this.setEditUrl(editUrl_);
                if(gradingViewUrl_)
                    this.setGradingViewUrl(gradingViewUrl_);
                if(viewUrl_)
                    this.setViewUrl(viewUrl_);
                if (banInfo_)
                    this.setBanInfo(banInfo_);
            }
        ]);
});
