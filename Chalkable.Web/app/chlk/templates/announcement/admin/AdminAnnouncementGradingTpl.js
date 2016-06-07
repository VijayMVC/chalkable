REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates.announcement.admin', function () {

    /** @class chlk.templates.announcement.admin.AdminAnnouncementGradingTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/admin/AdminAnnouncementGrading.jade')],
        'AdminAnnouncementGradingTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'items',

            ArrayOf(chlk.models.apps.AppAttachment), 'applications',

            ArrayOf(chlk.models.standard.Standard), 'standards',

            String, function getStandardsUrlComponents() {
                return (this.standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&');
            }
        ])
});