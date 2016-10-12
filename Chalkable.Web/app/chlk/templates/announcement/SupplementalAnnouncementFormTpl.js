REQUIRE('chlk.templates.announcement.AnnouncementFormTpl');
REQUIRE('chlk.models.announcement.AnnouncementForm');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    /** @class chlk.templates.announcement.SupplementalAnnouncementFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/SupplementalAnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'SupplementalAnnouncementFormTpl', EXTENDS(chlk.templates.announcement.AnnouncementFormTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'students'
        ]);
});