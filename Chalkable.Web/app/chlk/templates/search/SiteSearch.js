REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.search.SearchItem');
REQUIRE('chlk.models.course.Course');

NAMESPACE('chlk.templates.search', function () {

    /** @class chlk.templates.search.SiteSearch*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/sidebars/SiteSearch.jade')],
        [ria.templates.ModelBind(chlk.models.search.SearchItem)],
        'SiteSearch', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'announcementId',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            Number, 'searchType',

            [ria.templates.ModelPropertyBind],
            Number, 'roleId',

            [ria.templates.ModelPropertyBind],
            String, 'gender',

            [ria.templates.ModelPropertyBind],
            chlk.models.course.Course, 'course',

            [ria.templates.ModelPropertyBind],
            Number, 'announcementType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'adminAnnouncement',

            [ria.templates.ModelPropertyBind],
            Number, 'chalkableAnnouncementType'

        ])
});

