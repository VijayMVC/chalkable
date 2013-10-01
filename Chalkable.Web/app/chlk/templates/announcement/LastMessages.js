REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.announcement.LastMessages');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.LastMessages*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LastMessages.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.LastMessages)],
        'LastMessages', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Array, 'items',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName'
        ])
});