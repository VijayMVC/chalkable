REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.StandardService');

REQUIRE('chlk.models.announcement.AddStandardViewData');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.standard.StandardsListViewData');
REQUIRE('chlk.models.standard.StandardsTableViewData');
REQUIRE('chlk.models.standard.GetStandardTreePostData');

REQUIRE('chlk.activities.announcement.AddStandardsDialog');

NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.StandardController */
    CLASS(
        'StandardController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.StandardService, 'standardService',

            function showStandardsAction(){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
                var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);

                var res = ria.async.wait([
                    standardIds.length ? this.standardService.getStandardsList(standardIds) : ria.async.Future.$fromData(null),
                    this.standardService.getSubjects(options.getClassId())
                ]).attach(this.validateResponse_())
                    .then(function(result){
                        var selected = result[0] || [];
                        var subjects = result[1];
                        var breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.MAIN, 'Subjects');
                        return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.SUBJECT, options, [breadcrumb], standardIds, selected);
                    }, this);

                return this.ShadeView(chlk.activities.announcement.AddStandardsDialog, res);
            },

            [[chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId]],
            function showChildItemsAction(type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_){
                return this.showItemsAction(false, type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_);
            },

            [[chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId]],
            function showForBreadcrumbAction(type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_){
                return this.showItemsAction(true, type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_);
            },

            [[Boolean, chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId]],
            function showItemsAction(isBreadcrumb, type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
                var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null), res;

                if(!isBreadcrumb){
                    var breadcrumb = new chlk.models.standard.Breadcrumb(type, name, subjectId_, standardId_);
                    this.BackgroundUpdateView(chlk.activities.announcement.AddStandardsDialog, breadcrumb);
                }

                switch (type){
                    case chlk.models.standard.ItemType.MAIN:
                        res = this.getSubjectItems(options, standardIds);break;
                    case chlk.models.standard.ItemType.SUBJECT:
                    case chlk.models.standard.ItemType.STANDARD:
                        res = this.getStandards(options, standardIds, subjectId_, standardId_);break;
                }

                return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res, 'list-update');
            },

            function getSubjectItems(options, standardIds){
                return this.standardService.getSubjects(options.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(subjects){
                        return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.SUBJECT, options);
                    }, this);
            },

            function getStandards(options, standardIds, subjectId, standardId_){
                return this.standardService.getStandards(options.getClassId(), subjectId, standardId_)
                    .attach(this.validateResponse_())
                    .then(function(subjects){
                        return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.STANDARD, options);
                    }, this);
            },

            /*[chlk.controllers.NotChangedSidebarButton()],
            function showStandardsAction(){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
                var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);
                var res = this.standardService.getSubjects(options.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(subjects){
                        return new chlk.models.announcement.AddStandardViewData(options, subjects, standardIds);
                    }, this);
                return this.ShadeView(chlk.activities.announcement.AddStandardsDialog, res);
            },*/

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, String, chlk.models.id.StandardId]],
            function showStandardsByCategoryAction(classId, subjectId, description_, standardId_){
                var res = this.standardService.getStandardColumn(classId, subjectId, standardId_)
                    .attach(this.validateResponse_())
                    .then(function(standards){
                        var standardTable = new chlk.models.standard.StandardsTable.$createOneColumnTable(standards);
                        return new chlk.models.standard.StandardsTableViewData(description_, classId, subjectId, standardTable);
                    });
                return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.standard.GetStandardTreePostData]],
            function getStandardTreeAction(data){
                var res = this.standardService.getStandardParentsSubTree(data.getStandardId(), data.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(standardsTable){
                        var description, subjectId;
                        if(standardsTable && standardsTable.getStandardsColumns() && standardsTable.getStandardsColumns().length > 0){
                            var columns = standardsTable.getStandardsColumns();
                            var subjectId = columns[0][0].getSubjectId(), index = columns.length - 1;
                            while(index >= 0 && !description){
                                var lastSelected = columns[index].filter(function (s){return s.isSelected();});
                                if(lastSelected.length > 0){
                                    description = lastSelected[0].getDescription();
                                    standardsTable.addColumn([]);
                                }
                                index--;
                            }
                        }
                        return new chlk.models.standard.StandardsTableViewData(
                            description,
                            data.getClassId(),
                            subjectId,
                            standardsTable,
                            data.getAnnouncementId()
                        );
                    }, this);
                return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res, 'rebuild-standard-tree');
            }

        ]);
});