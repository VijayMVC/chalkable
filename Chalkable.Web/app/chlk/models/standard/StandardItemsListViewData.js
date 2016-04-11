REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.ABAuthorityId');
REQUIRE('chlk.models.id.ABDocumentId');
REQUIRE('chlk.models.id.ABSubjectDocumentId');
REQUIRE('chlk.models.id.ABStandardId');
REQUIRE('chlk.models.id.ABTopicId');
REQUIRE('chlk.models.id.ABCourseId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    /** @class chlk.models.standard.ItemType*/
    ENUM('ItemType', {
        MAIN: 0,
        SUBJECT: 1,
        STANDARD: 2,
        AB_MAIN: 3,
        AUTHORITY: 4,
        DOCUMENT: 5,
        SUBJECT_DOCUMENT: 6,
        GRADE_LEVEL: 7,
        STANDARD_COURSE: 8,
        AB_STANDARD: 9,
        TOPIC_MAIN: 10,
        TOPIC_SUBJECT: 11,
        TOPIC_COURSE: 12,
        TOPIC: 13,
        SEARCH: 14
    });

    /** @class chlk.models.standard.Breadcrumb*/
    CLASS(
        'Breadcrumb', [
            String, 'name',

            chlk.models.standard.ItemType, 'type',

            chlk.models.id.StandardSubjectId, 'subjectId',

            chlk.models.id.StandardId, 'standardId',

            chlk.models.id.ABAuthorityId, 'authorityId',

            chlk.models.id.ABDocumentId, 'documentId',

            chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',

            String, 'gradeLevelCode',

            chlk.models.id.ABStandardId, 'ABStandardId',

            chlk.models.id.ABTopicId, 'topicId',

            chlk.models.id.ABCourseId, 'standardCourseId',

            chlk.models.id.ABCourseId, 'courseId',

            [[chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId,
                chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABCourseId, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function $(type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_){
                BASE();
                this.setType(type);
                this.setName(name);
                subjectId_ && this.setSubjectId(subjectId_);
                standardId_ && this.setStandardId(standardId_);
                authorityId_ && this.setAuthorityId(authorityId_);
                documentId_ && this.setDocumentId(documentId_);
                subjectDocumentId_ && this.setSubjectDocumentId(subjectDocumentId_);
                gradeLevelCode_ && this.setGradeLevelCode(gradeLevelCode_);
                standardCourseId_ && this.setStandardCourseId(standardCourseId_);
                ABStandardId_ && this.setABStandardId(ABStandardId_);
                courseId_ && this.setCourseId(courseId_);
                topicId_ && this.setTopicId(topicId_);
            }
        ]);

    /** @class chlk.models.standard.StandardItemsListViewData*/
    CLASS(
        'StandardItemsListViewData', [
            chlk.models.standard.ItemType, 'currentItemsType',

            Array, 'itemIds',

            String, 'requestId',

            chlk.models.common.AttachOptionsViewData, 'attachOptions',

            Array, 'items',

            Array, 'selectedItems',

            Boolean, 'onlyOne',

            ArrayOf(chlk.models.standard.Breadcrumb), 'breadcrumbs',

            [[Array, chlk.models.standard.ItemType, chlk.models.common.AttachOptionsViewData, ArrayOf(chlk.models.standard.Breadcrumb), Array, Array, String, Boolean]],
            function $(items, itemsType, attachOptions_, breadcrumbs_, itemIds_, selected_, requestId_, onlyOne_){
                BASE();
                this.updateByValues(items, itemsType, attachOptions_, breadcrumbs_, itemIds_, selected_, requestId_, onlyOne_);
            },

            [[Array, chlk.models.standard.ItemType, chlk.models.common.AttachOptionsViewData, ArrayOf(chlk.models.standard.Breadcrumb), Array, Array, String, Boolean]],
            function updateByValues(items_, itemsType_, attachOptions_, breadcrumbs_, itemIds_, selected_, requestId_, onlyOne_){
                attachOptions_ && this.setAttachOptions(attachOptions_);
                itemsType_ && this.setCurrentItemsType(itemsType_);
                items_ && this.setItems(items_);
                breadcrumbs_ && this.setBreadcrumbs(breadcrumbs_);
                itemIds_ && this.setItemIds(itemIds_);
                selected_ && this.setSelectedItems(selected_);
                requestId_ && this.setRequestId(requestId_);
                onlyOne_ && this.setOnlyOne(onlyOne_);
            }
        ]);
});
