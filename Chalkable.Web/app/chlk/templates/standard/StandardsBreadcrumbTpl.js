REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.StandardItemsListViewData');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.StandardsBreadcrumbTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/standard/StandardsBreadcrumb.jade')],
        [ria.templates.ModelBind(chlk.models.standard.Breadcrumb)],
        'StandardsBreadcrumbTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.standard.ItemType, 'type',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardSubjectId, 'subjectId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABAuthorityId, 'authorityId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABDocumentId, 'documentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',

            [ria.templates.ModelPropertyBind],
            String, 'gradeLevelCode',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABCourseId, 'standardCourseId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABStandardId, 'ABStandardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABTopicId, 'topicId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ABCourseId, 'courseId'
    ]);
});