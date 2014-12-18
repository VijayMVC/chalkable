REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AddStandardViewData');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AddStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AddMathStandards.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AddStandardViewData)],
        'AddStandardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Array, 'standardIds',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.StandardSubject), 'itemStandards'
    ]);
});