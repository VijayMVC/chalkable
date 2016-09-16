NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementTitleViewData*/
    CLASS(
        'AnnouncementTitleViewData', [
            [[String, chlk.models.announcement.AnnouncementTypeEnum]],
            function $(title_, type_){
                BASE();
                if(title_)
                    this.setTitle(title_);
                if(type_)
                    this.setType(type_);
            },

            String, 'title',

            chlk.models.announcement.AnnouncementTypeEnum, 'type'
        ]);
});
