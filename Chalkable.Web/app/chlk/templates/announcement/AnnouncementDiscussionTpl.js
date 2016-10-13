REQUIRE('chlk.templates.announcement.AnnouncementView');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementDiscussionTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementDiscussion.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementView)],
        'AnnouncementDiscussionTpl', EXTENDS(chlk.templates.announcement.AnnouncementView), [
            function getCount_(comments){
                var count = 0, that = this;
                comments.forEach(function(comment){
                    count+=1;
                    if(comment.getSubComments())
                        count += that.getCount_(comment.getSubComments());
                });
                return count;
            },

            function getCommentsCount(){
                return this.getCount_(this.getAnnouncementComments());
            }
        ])
});