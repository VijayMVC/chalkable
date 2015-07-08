REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementAttributeType');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";
    /** @class chlk.templates.announcement.AnnouncementAttributesDropdownTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAttributesDropdown.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementAttributeType)],
        'AnnouncementAttributesDropdownTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAttributeTypeId, 'id'
        ]);
});
