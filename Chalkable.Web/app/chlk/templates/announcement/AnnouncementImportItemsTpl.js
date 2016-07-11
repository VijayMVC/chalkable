REQUIRE('chlk.templates.announcement.AnnouncementImportTpl');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementImportItemsTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementImportViewData)],
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementImportItems.jade')],
        'AnnouncementImportItemsTpl', EXTENDS(chlk.templates.announcement.AnnouncementImportTpl), [
            function getDateText(item){
                if(item.getExpiresDate)
                    return item.getExpiresDate().toStandardFormat();

                return (item.getStartDate() ? item.getStartDate().toStandardFormat() : '') + ' : ' + (item.getEndDate() ? item.getEndDate().toStandardFormat() : '')
            }
        ])
});