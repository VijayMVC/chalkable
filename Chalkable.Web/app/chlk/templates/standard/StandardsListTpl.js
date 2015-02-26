REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.StandardsListViewData');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.StandardsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/StandardsColumns.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardsListViewData)],
        'StandardsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardSubjectId, 'subjectId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'itemStandards'
    ]);
});