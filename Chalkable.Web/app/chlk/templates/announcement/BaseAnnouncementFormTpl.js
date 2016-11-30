REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.AnnouncementTitleViewData');
REQUIRE('chlk.models.apps.SuggestedAppsList');
REQUIRE('chlk.models.standard.StandardsListViewData');

REQUIRE('chlk.templates.standard.AnnouncementStandardsTpl');


NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    /** @class chlk.templates.announcement.BaseAnnouncementFormTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementCreate)],
        'BaseAnnouncementFormTpl', EXTENDS(chlk.templates.ChlkTemplate), [



            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.FeedAnnouncementViewData, 'announcement',

            [ria.templates.ModelPropertyBind],
            Boolean, 'isDraft'

        ]);
});