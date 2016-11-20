REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.StandardService');
REQUIRE('chlk.services.ABStandardService');

REQUIRE('chlk.models.announcement.AddStandardViewData');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.standard.StandardsListViewData');
REQUIRE('chlk.models.standard.GetStandardTreePostData');

REQUIRE('chlk.activities.announcement.AddStandardsDialog');
REQUIRE('chlk.activities.apps.AddTopicDialog');
REQUIRE('chlk.activities.apps.AddABStandardDialog');

NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.StandardController */
    CLASS(
        'StandardController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.StandardService, 'standardService',

            [ria.mvc.Inject],
            chlk.services.ABStandardService, 'ABStandardService',

            [[chlk.models.id.ClassId]],
            function showStandardsAction(){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []),
                    argsObj = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_LAST_ARGUMENTS, {}),
                    breadcrumbsObj = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_BREADCRUMBS, {}),
                    args, breadcrumbs, options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);

                var classId = options.getClassId();

                if(argsObj[classId.valueOf()]){
                    args = argsObj[classId.valueOf()];
                }else{
                    args = [false, chlk.models.standard.ItemType.MAIN, 'Subjects'];
                }

                if(breadcrumbsObj[classId.valueOf()]){
                    breadcrumbs = breadcrumbsObj[classId.valueOf()];
                }else{
                    breadcrumbs = [new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.MAIN, 'Subjects')];
                    breadcrumbsObj[classId.valueOf()] = breadcrumbs;
                    this.getContext().getSession().set(ChlkSessionConstants.STANDARD_BREADCRUMBS, breadcrumbsObj);
                }

                var res = ria.async.wait([
                    standardIds.length ? this.standardService.getStandardsList(standardIds) : ria.async.Future.$fromData(null),
                    this.setItemsByArgs.apply(this, args)
                ]).attach(this.validateResponse_())
                    .then(function(result){
                        var selected = result[0] || [];
                        var model = result[1];
                        model.updateByValues(null, null, null, breadcrumbs, standardIds, selected);
                        return model;
                    }, this);

                return this.ShadeView(chlk.activities.announcement.AddStandardsDialog, res);
            },

            [[chlk.models.standard.ItemType, String, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABCourseId, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function showChildItemsAction(type, name, description_, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_){
                return this.showItemsAction(false, type, name, description_, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_);
            },

            [[chlk.models.standard.ItemType, String, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABCourseId, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function showForBreadcrumbAction(type, name, description_, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_){
                return this.showItemsAction(true, type, name, description_, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_);
            },

            [[Boolean, chlk.models.standard.ItemType, String, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABCourseId, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function showItemsAction(isBreadcrumb, type, name, description_, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_){
                var res = this.setItemsByArgs.apply(this, arguments), pickerType, BCCount, BCType, AType;

                switch (type){
                    case chlk.models.standard.ItemType.MAIN:
                        pickerType = 1; BCCount = 1;break;
                    case chlk.models.standard.ItemType.SUBJECT:
                        pickerType = 1; BCCount = 2;break;
                    case chlk.models.standard.ItemType.STANDARD:
                        pickerType = 1; BCCount = 3;break;
                    case chlk.models.standard.ItemType.AB_MAIN:
                        pickerType = 2; BCCount = 1;break;
                    case chlk.models.standard.ItemType.AUTHORITY:
                        pickerType = 2; BCCount = 2;break;
                    case chlk.models.standard.ItemType.DOCUMENT:
                        pickerType = 2; BCCount = 3;break;
                    case chlk.models.standard.ItemType.SUBJECT_DOCUMENT:
                        pickerType = 2; BCCount = 4;break;
                    case chlk.models.standard.ItemType.GRADE_LEVEL:
                        pickerType = 2; BCCount = 5;break;
                    case chlk.models.standard.ItemType.STANDARD_COURSE:
                        pickerType = 2; BCCount = 6;break;
                    case chlk.models.standard.ItemType.AB_STANDARD:
                        pickerType = 2; BCCount = 7;break;
                    case chlk.models.standard.ItemType.TOPIC_MAIN:
                        pickerType = 3; BCCount = 1;break;
                    case chlk.models.standard.ItemType.TOPIC_SUBJECT:
                        pickerType = 3; BCCount = 2;break;
                    case chlk.models.standard.ItemType.TOPIC_COURSE:
                        pickerType = 3; BCCount = 3;break;
                    case chlk.models.standard.ItemType.TOPIC:
                        pickerType = 3; BCCount = 4;break;
                }

                switch (pickerType){
                    case 1: BCType = ChlkSessionConstants.STANDARD_BREADCRUMBS; AType = ChlkSessionConstants.STANDARD_LAST_ARGUMENTS; break;
                    case 2: BCType = ChlkSessionConstants.AB_STANDARD_BREADCRUMBS; AType = ChlkSessionConstants.AB_STANDARD_LAST_ARGUMENTS; break;
                    case 3: BCType = ChlkSessionConstants.TOPIC_BREADCRUMBS; AType = ChlkSessionConstants.TOPIC_LAST_ARGUMENTS; break;
                }

                var breadcrumbs, breadcrumbsObj;

                if(pickerType == 1){
                    var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);
                    var classId = options.getClassId();

                    var argsObj = this.getContext().getSession().get(AType, {});
                    argsObj[classId.valueOf()] = arguments;
                    this.getContext().getSession().set(AType, argsObj);
                    breadcrumbsObj = this.getContext().getSession().get(BCType, {});

                    breadcrumbs = breadcrumbsObj[classId.valueOf()] || [];
                }else{
                    this.getContext().getSession().set(AType, arguments);
                    breadcrumbs = this.getContext().getSession().get(BCType, []);
                }

                if(isBreadcrumb){
                    breadcrumbs = breadcrumbs.slice(0, BCCount);
                }else{
                    var breadcrumb = new chlk.models.standard.Breadcrumb(type, name, description_, subjectId_, standardId_, authorityId_, documentId_,
                        subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_);
                    breadcrumbs.push(breadcrumb);
                }

                if(pickerType == 1){
                    breadcrumbsObj[classId.valueOf()] = breadcrumbs;
                    this.getContext().getSession().set(BCType, breadcrumbsObj);
                }else{
                    this.getContext().getSession().set(BCType, breadcrumbs);
                }

                res = res.then(function(model){
                    if(!isBreadcrumb){
                        this.BackgroundUpdateView(this.getView().getCurrent().getClass(), breadcrumb, 'add-breadcrumb');
                    }

                    return model;
                }.bind(this));

                return this.UpdateView(this.getView().getCurrent().getClass(), res, 'list-update');
            },

            [[Boolean, chlk.models.standard.ItemType, String, String, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId, chlk.models.id.ABAuthorityId,
                chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String, chlk.models.id.ABCourseId, chlk.models.id.ABStandardId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId]],
            function setItemsByArgs(isBreadcrumb, type, name, description_, subjectId_, standardId_, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, courseId_, topicId_){
                var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []),
                    options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null), res;

                switch (type){
                    case chlk.models.standard.ItemType.MAIN:
                        res = this.getSubjectItems(options);break;
                    case chlk.models.standard.ItemType.SUBJECT:
                    case chlk.models.standard.ItemType.STANDARD:
                        res = this.getStandards(options, standardIds, subjectId_, standardId_, name, description_);break;
                    case chlk.models.standard.ItemType.AB_MAIN:
                        res = this.getAuthoritiesItems(options);break;
                    case chlk.models.standard.ItemType.AUTHORITY:
                        res = this.getDocumentsItems(options, authorityId_);break;
                    case chlk.models.standard.ItemType.DOCUMENT:
                        res = this.getSubjectDocumentsItems(options, authorityId_, documentId_);break;
                    case chlk.models.standard.ItemType.SUBJECT_DOCUMENT:
                        res = this.getGradeLevelsItems(options, authorityId_, documentId_, subjectDocumentId_);break;
                    case chlk.models.standard.ItemType.GRADE_LEVEL:
                        res = this.getStandardCourseItems(options, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_);break;
                    case chlk.models.standard.ItemType.STANDARD_COURSE:
                    case chlk.models.standard.ItemType.AB_STANDARD:
                        res = this.getABStandardsItems(options, authorityId_, documentId_, subjectDocumentId_, gradeLevelCode_, standardCourseId_, ABStandardId_, name, description_);break;
                    case chlk.models.standard.ItemType.TOPIC_MAIN:
                        res = this.getSubjectDocumentsItems(options, null, null, true);break;
                    case chlk.models.standard.ItemType.TOPIC_SUBJECT:
                        res = this.getTopicCourses(options, subjectDocumentId_);break;
                    case chlk.models.standard.ItemType.TOPIC_COURSE:
                    case chlk.models.standard.ItemType.TOPIC:
                        res = this.getTopics(options, subjectDocumentId_, courseId_, topicId_, name);break;
                }

                return res;
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABSubjectDocumentId]],
            function getTopicCourses(options, subjectDocumentId_){
                return this.ABStandardService.getTopicCourses(subjectDocumentId_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.TOPIC_COURSE, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABSubjectDocumentId, chlk.models.id.ABCourseId, chlk.models.id.ABTopicId, String]],
            function getTopics(options, subjectDocumentId_, courseId_, topicId_, name){
                return this.ABStandardService.getTopics(subjectDocumentId_, courseId_, topicId_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        if(topicId_ && topicId_.valueOf())
                            items.unshift(new chlk.models.academicBenchmark.Topic(topicId_, name, true));

                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.TOPIC, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData]],
            function getAuthoritiesItems(options){
                return this.ABStandardService.getAuthorities()
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.AUTHORITY, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABAuthorityId]],
            function getDocumentsItems(options, authorityId){
                return this.ABStandardService.getDocuments(authorityId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.DOCUMENT, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, Boolean]],
            function getSubjectDocumentsItems(options, authorityId, documentId, forTopics_){
                var res = forTopics_
                    ? this.ABStandardService.getTopicSubjectDocuments()
                    : this.ABStandardService.getSubjectDocuments(authorityId, documentId);

                return res
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, forTopics_ ? chlk.models.standard.ItemType.TOPIC_SUBJECT :
                            chlk.models.standard.ItemType.SUBJECT_DOCUMENT, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId]],
            function getGradeLevelsItems(options, authorityId, documentId, subjectDocumentId){
                return this.ABStandardService.getGradeLevels(authorityId, documentId, subjectDocumentId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.GRADE_LEVEL, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String]],
            function getStandardCourseItems(options, authorityId, documentId, subjectDocumentId, gradeLevelCode_){
                return this.ABStandardService.getStandardCourses(authorityId, documentId, subjectDocumentId, gradeLevelCode_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.STANDARD_COURSE, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData, chlk.models.id.ABAuthorityId, chlk.models.id.ABDocumentId, chlk.models.id.ABSubjectDocumentId, String,
                chlk.models.id.ABCourseId, chlk.models.id.ABStandardId, String, String]],
            function getABStandardsItems(options, authorityId, documentId, subjectDocumentId, gradeLevelCode_, standardCourseId_, ABStandardId_, name, description_){
                return this.ABStandardService.getStandards(authorityId, documentId, subjectDocumentId, gradeLevelCode_, standardCourseId_, ABStandardId_, true)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        if(ABStandardId_ && ABStandardId_.valueOf())
                            items.unshift(new chlk.models.academicBenchmark.Standard(ABStandardId_, name, description_, true));

                        return new chlk.models.standard.StandardItemsListViewData(items, chlk.models.standard.ItemType.AB_STANDARD, options);
                    }, this);
            },

            [[chlk.models.common.AttachOptionsViewData]],
            function getSubjectItems(options){
                return this.standardService.getSubjects(options.getClassId())
                    .attach(this.validateResponse_())
                    .then(function(subjects){
                        return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.SUBJECT, options);
                    }, this);
            },

            function getStandards(options, standardIds, subjectId, standardId_, name, description_){
                return this.standardService.getStandards(options.getClassId(), subjectId, standardId_)
                    .attach(this.validateResponse_())
                    .then(function(standards){
                        if(standardId_ && standardId_.valueOf())
                            standards.unshift(new chlk.models.standard.Standard(standardId_, name, null, description_, true));

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
                var args = this.getContext().getSession().get(ChlkSessionConstants.TOPIC_LAST_ARGUMENTS, [false, chlk.models.standard.ItemType.TOPIC_MAIN, 'Topic']),
                    breadcrumbs = this.getContext().getSession().get(ChlkSessionConstants.TOPIC_BREADCRUMBS, null);

                if(!breadcrumbs){
                    breadcrumbs = [new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.TOPIC_MAIN, 'Topic')];
                    this.getContext().getSession().set(ChlkSessionConstants.TOPIC_BREADCRUMBS, breadcrumbs);
                }

                var res = ria.async.wait([
                    topicsIds.length ? this.ABStandardService.getTopicsList(topicsIds) : ria.async.Future.$fromData(null),
                    this.setItemsByArgs.apply(this, args)
                ]).attach(this.validateResponse_())
                    .then(function(result){
                        var selected = result[0] || [];
                        var model = result[1];
                        model.updateByValues(null, null, null, breadcrumbs, topicsIds, selected, requestId, onlyOne_);
                        return model;
                    }, this);

                return this.ShadeView(chlk.activities.apps.AddTopicDialog, res)
            },

            function completeTopicsWidgetAction(model){
                this.getContext().getSession().remove(ChlkSessionConstants.TOPIC_BREADCRUMBS);
                this.getContext().getSession().remove(ChlkSessionConstants.TOPIC_LAST_ARGUMENTS);

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
                    var result = this.ABStandardService.getTopicSubjectDocuments()
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
            },

            //AB STANDARDS

            [[String, Array, Boolean]],
            function showABStandardsWidgetAction(requestId, standardIds, onlyOne_){
                var args = this.getContext().getSession().get(ChlkSessionConstants.AB_STANDARD_LAST_ARGUMENTS, [false, chlk.models.standard.ItemType.AB_MAIN, 'Source']),
                    breadcrumbs = this.getContext().getSession().get(ChlkSessionConstants.AB_STANDARD_BREADCRUMBS, null);

                if(!breadcrumbs){
                    breadcrumbs = [new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.AB_MAIN, 'Source')];
                    this.getContext().getSession().set(ChlkSessionConstants.AB_STANDARD_BREADCRUMBS, breadcrumbs);
                }

                var res = ria.async.wait([
                    standardIds.length ? this.ABStandardService.getStandardsList(standardIds) : ria.async.Future.$fromData(null),
                    this.setItemsByArgs.apply(this, args)
                ]).attach(this.validateResponse_())
                    .then(function(result){
                        var selected = result[0] || [];
                        var model = result[1];
                        model.updateByValues(null, null, null, breadcrumbs, standardIds, selected, requestId, onlyOne_);
                        return model;
                    }, this);

                return this.ShadeView(chlk.activities.apps.AddABStandardDialog, res)
            },

            function completeABStandardsWidgetAction(model){
                this.getContext().getSession().remove(ChlkSessionConstants.AB_STANDARD_BREADCRUMBS);
                this.getContext().getSession().remove(ChlkSessionConstants.AB_STANDARD_LAST_ARGUMENTS);
                var stIds = model.standardIds ? model.standardIds.split(',').filter(function(item){return item}) : [];
                if(stIds.length)
                    this.ABStandardService.getStandardsList(stIds)
                        .then(function(data){
                            this.WidgetComplete(model.requestId, data);
                        }, this);
                else
                    this.WidgetComplete(model.requestId, []);

                return this.CloseView(chlk.activities.apps.AddABStandardDialog);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.standard.GetStandardTreePostData]],
            function searchABStandardsAction(data){
                var breadcrumb;

                if(!data.getFilter()){
                    var result = this.ABStandardService.getAuthorities()
                        .attach(this.validateResponse_())
                        .then(function(subjects){
                            breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.AB_MAIN, 'Source');
                            return new chlk.models.standard.StandardItemsListViewData(subjects, chlk.models.standard.ItemType.AUTHORITY, null, [breadcrumb]);
                        }, this);

                    return this.UpdateView(this.getView().getCurrent().getClass(), result, 'clear-search');
                }

                var res = this.ABStandardService.searchStandards(data.getFilter())
                    .attach(this.validateResponse_())
                    .then(function(standards){
                        breadcrumb = new chlk.models.standard.Breadcrumb(chlk.models.standard.ItemType.SEARCH, 'Standards');
                        this.BackgroundUpdateView(this.getView().getCurrent().getClass(), breadcrumb, 'replace-breadcrumbs');

                        return new chlk.models.standard.StandardItemsListViewData(standards, chlk.models.standard.ItemType.AB_STANDARD);
                    }, this);

                return this.UpdateView(this.getView().getCurrent().getClass(), res, 'list-update');
            }

        ]);
});