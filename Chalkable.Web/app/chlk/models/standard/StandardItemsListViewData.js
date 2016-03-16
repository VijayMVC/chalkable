REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.ABAuthorityId');
REQUIRE('chlk.models.id.ABDocumentId');
REQUIRE('chlk.models.id.ABSubjectDocumentId');
REQUIRE('chlk.models.id.ABStandardId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    /** @class chlk.models.standard.ItemType*/
    ENUM('ItemType', {
        MAIN: 0,
        SUBJECT: 1,
        STANDARD: 2,
        AUTHORITY: 3,
        DOCUMENT: 4,
        SUBJECT_DOCUMENT: 5,
        GRADE_LEVEL: 6,
        AB_STANDARD: 7,
        AB_MAIN: 8
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

            [[chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId]],
            function $(type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_){
                BASE();
                this.setType(type);
                this.setName(name);
                subjectId_ && this.setSubjectId(subjectId_);
                standardId_ && this.setStandardId(standardId_);
                authorityId_ && this.setAuthorityId(authorityId_);
                documentId_ && this.setDocumentId(documentId_);
                subjectDocumentId_ && this.setSubjectDocumentId(subjectDocumentId_);
                gradeLevelCode_ && this.setGradeLevelCode(gradeLevelCode_);
                ABStandardId_ && this.setABStandardId(ABStandardId_);
            }
        ]);

    /** @class chlk.models.standard.StandardItemsListViewData*/
    CLASS(
        'StandardItemsListViewData', [
            chlk.models.standard.ItemType, 'currentItemsType',

            Array, 'standardIds',

            chlk.models.common.AttachOptionsViewData, 'attachOptions',

            Array, 'items',

            Array, 'selectedStandards',

            ArrayOf(chlk.models.standard.Breadcrumb), 'breadcrumbs',

            [[Array, chlk.models.standard.ItemType, chlk.models.common.AttachOptionsViewData, ArrayOf(chlk.models.standard.Breadcrumb), Array, Array]],
            function $(items, itemsType, attachOptions, breadcrumbs_, standardIds_, selected_){
                BASE();
                this.setAttachOptions(attachOptions);
                this.setCurrentItemsType(itemsType);
                this.setItems(items);
                breadcrumbs_ && this.setBreadcrumbs(breadcrumbs_);
                standardIds_ && this.setStandardIds(standardIds_);
                selected_ && this.setSelectedStandards(selected_);
            }
        ]);
});
