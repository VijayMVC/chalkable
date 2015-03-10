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
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'BaseAnnouncementFormTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.Announcement, 'announcement',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'isDraft',

            [ria.templates.ModelPropertyBind],
            Array, 'classScheduleDateRanges',

            chlk.models.standard.StandardsListViewData, function prepareStandardsListData() {
                var ann = this.announcement;
                return new chlk.models.standard.StandardsListViewData(
                    null, ann.getClassId(),
                    null, ann.getStandards(),
                    ann.getId()
                );
            }
        ]);
});