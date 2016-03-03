REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.AppContentListViewData');

NAMESPACE('chlk.templates.apps', function() {
    "use strict";

    /**@class chlk.templates.apps.AppContentListTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppContentListView.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppContentListViewData)],
        'AppContentListTpl', EXTENDS(chlk.templates.ChlkTemplate), [


            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'application',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'appContents',

            Boolean, function hasContent(){
                return this.getAppContents() && this.getAppContents().getItems() && this.getAppContents().getItems().length > 0;
            }
        ]);
});