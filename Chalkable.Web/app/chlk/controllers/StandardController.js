REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.StandardService');
REQUIRE('chlk.services.ABStandardService');

REQUIRE('chlk.models.announcement.AddStandardViewData');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.standard.StandardsListViewData');
REQUIRE('chlk.models.standard.GetStandardTreePostData');

REQUIRE('chlk.activities.announcement.AddStandardsDialog');
REQUIRE('chlk.activities.apps.AddTopicDialog');

NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.StandardController */
    CLASS(
        'StandardController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.StandardService, 'standardService',

            [ria.mvc.Inject],
            chlk.services.ABStandardService, 'ABStandardService',

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
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function showChildItemsAction(type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_, courseId_, topicId_){
                return this.showItemsAction(false, type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_, courseId_, topicId_);
            },

            [[chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function showForBreadcrumbAction(type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_, courseId_, topicId_){
                return this.showItemsAction(true, type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_, courseId_, topicId_);
            },

            [[Boolean, chlk.models.standard.ItemType, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function showItemsAction(isBreadcrumb, type, name, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_, courseId_, topicId_){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
                var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null), res;

                switch (type){
                    case chlk.models.standard.ItemType.MAIN:
                        res = this.getSubjectItems(options);break;
                    case chlk.models.standard.ItemType.SUBJECT:
                    case chlk.models.standard.ItemType.STANDARD:
                        res = this.getStandards(options, standardIds, subjectId_, standardId_);break;
                    case chlk.models.standard.ItemType.AB_MAIN:
                        res = this.getAuthoritiesItems();break;
                    case chlk.models.standard.ItemType.AUTHORITY:
                        res = this.getDocumentsItems(authorityId_);break;
                    case chlk.models.standard.ItemType.DOCUMENT:
                        res = this.getSubjectDocumentsItems(authorityId_, documentId_);break;
                    case chlk.models.standard.ItemType.SUBJECT_DOCUMENT:
                        res = this.getGradeLevelsItems(authorityId_, documentId_, subjectDocumentId_);break;
                    case chlk.models.standard.ItemType.GRADE_LEVEL:
                    case chlk.models.standard.ItemType.AB_STANDARD:
                        res = this.getABStandardsItems(authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, ABStandardId_);break;
                    case chlk.models.standard.ItemType.TOPIC_MAIN:
                        res = this.getSubjectDocumentsItems(null, null, true);break;
                    case chlk.models.standard.ItemType.TOPIC_SUBJECT:
                        res = this.getTopicCourses(subjectDocumentId_);break;
                    case chlk.models.standard.ItemType.TOPIC_COURSE:
                    case chlk.models.standard.ItemType.TOPIC:
                        res = this.getTopics(subjectDocumentId_, courseId_, topicId_);break;
                }

                res = res.then(function(model){
                    if(!isBreadcrumb){
                        var breadcrumb = new chlk.models.standard.Breadcrumb(type, name, subjectId_, standardId_, authorityId_, documentId_,
                            subjectDocumentId_, gradeLevelCode_, ABStandardId_, courseId_, topicId_);
                        this.BackgroundUpdateView(this.getView().getCurrent().getClass(), breadcrumb, 'add-breadcrumb');
                    }

                    return model;
                }.bind(this));

                return this.UpdateView(this.getView().getCurrent().getClass(), res, 'list-update');
            },

            function getTopicCourses(subjectDocumentId_){
                return this.ABStandardService.getCourses(subjectDocumentId_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.TOPIC_COURSE);
                    }, this);
            },

            [[chlk.models.id.ABSubjectDocumentId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function getTopics(subjectDocumentId_, courseId_, topicId_){
                return this.ABStandardService.getTopics(subjectDocumentId_, courseId_, topicId_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.TOPIC);
                    }, this);
            },

            function getAuthoritiesItems(){
                return this.ABStandardService.getAuthorities()
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.AUTHORITY);
                    }, this);
            },

            [[chlk.models.id.ABAuthorityId]],
            function getDocumentsItems(authorityId){
                return this.ABStandardService.getDocuments(authorityId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.DOCUMENT);
                    }, this);
            },

            [[chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, Boolean]],
            function getSubjectDocumentsItems(authorityId, documentId, forTopics_){
                return this.ABStandardService.getSubjectDocuments(authorityId, documentId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, forTopics_ ? chlk.models.standard.ItemType.TOPIC_SUBJECT :
                            chlk.models.standard.ItemType.SUBJECT_DOCUMENT);
                    }, this);
            },

            [[chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId]],
            function getGradeLevelsItems(authorityId, documentId, subjectDocumentId){
                return this.ABStandardService.getGradeLevels(authorityId, documentId, subjectDocumentId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.GRADE_LEVEL);
                    }, this);
            },

            [[chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABStandardId]],
            function getABStandardsItems(authorityId, documentId, subjectDocumentId, gradeLevelCode_, ABStandardId_){
                return this.ABStandardService.getStandards(authorityId, documentId, subjectDocumentId, gradeLevelCode_, ABStandardId_, true)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.AB_STANDARD);
                    }, this);
            },

            function getSubjectItems(options){
                return this.standardService.getSubjects(options.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(subjects){
                        return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.SUBJECT, options);
                    }, this);
            },

            function getStandards(options, standardIds, subjectId, standardId_){
                return this.standardService.getStandards(options.getClassId(), subjectId, standardId_)
                    .attach(this.validateResponse_())
                    .then(function(standards){
                        return new chlk.models.standard.StandardItemsListViewData(standards, chlk.models.standard.ItemType.STANDARD, options);
                    }, this);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.standard.GetStandardTreePostData]],
            function searchStandardsAction(data){
                var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null), breadcrumb;

                if(!data.getFilter()){
                    var result = this.standardService.getSubjects(data.getClassId())
                        .attach(this.validateResponse_())
                        .then(function(subjects){
                            breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.MAIN, 'Subjects');
                            return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.SUBJECT, options, [breadcrumb]);
                        }, this);

                    return this.UpdateView(this.getView().getCurrent().getClass(), result, 'clear-search');
                }

                var res = this.standardService.searchStandards(data.getFilter(), data.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(standards){
                        breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.SEARCH, 'Standards');
                        this.BackgroundUpdateView(this.getView().getCurrent().getClass(), breadcrumb, 'replace-breadcrumbs');
                        return new chlk.models.standard.StandardItemsListViewData(standards, chlk.models.standard.ItemType.STANDARD, options);
                    }, this);

                return this.UpdateView(this.getView().getCurrent().getClass(), res, 'list-update');
            },

            //TOPICS

            [[String, Array, Boolean]],
            function showTopicsWidgetAction(requestId, topicsIds, onlyOne_){
                var res = ria.async.wait([
                    topicsIds.length ? this.ABStandardService.getTopicsList(topicsIds) : ria.async.Future.$fromData(null),
                    this.ABStandardService.getSubjectDocuments()
                ]).attach(this.validateResponse_())
                    .then(function(result){
                        var selected = result[0] || [];
                        var subjects = result[1];
                        var breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.TOPIC_MAIN, 'Topic');
                        return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.TOPIC_SUBJECT,
                            null, [breadcrumb], topicsIds, selected, requestId, onlyOne_);
                    }, this);

                return this.ShadeView(chlk.activities.apps.AddTopicDialog, res)
            },

            function completeTopicsWidgetAction(model){
                var stIds = model.itemIds ? model.itemIds.split(',').filter(function(item){return item}) : [];
                if(stIds.length)
                    this.ABStandardService.getTopicsList(stIds)
                        .then(function(data){
                            this.WidgetComplete(model.requestId, data);
                        }, this);
                else
                    this.WidgetComplete(model.requestId, []);

                return this.CloseView(chlk.activities.apps.AddTopicDialog);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.standard.GetStandardTreePostData]],
            function searchTopicsAction(data){
                var breadcrumb;

                if(!data.getFilter()){
                    var result = this.ABStandardService.getSubjectDocuments()
                        .attach(this.validateResponse_())
                        .then(function(subjects){
                            breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.TOPIC_MAIN, 'Topic');
                            return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.TOPIC_SUBJECT, null, [breadcrumb]);
                        }, this);

                    return this.UpdateView(this.getView().getCurrent().getClass(), result, 'clear-search');
                }

                var res = this.ABStandardService.searchTopics(data.getFilter())
                    .attach(this.validateResponse_())
                    .then(function(topics){
                        breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.SEARCH, 'Topics');
                        this.BackgroundUpdateView(this.getView().getCurrent().getClass(), breadcrumb, 'replace-breadcrumbs');

                        return new chlk.models.standard.StandardItemsListViewData(topics, chlk.models.standard.ItemType.TOPIC);
                    }, this);

                return this.UpdateView(this.getView().getCurrent().getClass(), res, 'list-update');
            }

        ]);
});