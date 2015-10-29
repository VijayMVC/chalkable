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

            [chlk.controllers.NotChangedSidebarButton()],
            function showStandardsAction(){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
                var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);
                var res = this.standardService.getSubjects(options.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(subjects){
                        return new chlk.models.announcement.AddStandardViewData(options, subjects, standardIds);
                    }, this);
                return this.ShadeView(chlk.activities.announcement.AddStandardsDialog, res);
            },

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
                            var subjectId = columns[0][0].getSubjectId();
                            var lastSelected = columns[columns.length - 1].filter(function (s){return s.isSelected();});
                            if(lastSelected.length > 0){
                                description = lastSelected[0].getDescription();
                                standardsTable.addColumn([]);
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