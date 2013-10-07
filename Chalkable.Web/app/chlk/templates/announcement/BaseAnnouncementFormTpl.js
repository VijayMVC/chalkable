REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementForm');

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
            Array, 'reminders'
        ]);
});