NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementTitleViewData*/
    CLASS(
        'AnnouncementTitleViewData', [
            [[String]],
            function $(title_){
                BASE();
                if(title_)
                    this.setTitle(title_);
            },

            String, 'title'
        ]);
});
