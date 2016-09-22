REQUIRE('chlk.controls.Base');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListItemsTpl');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListTpl');
REQUIRE('chlk.templates.controls.group_people_selector.PersonItemsTpl');
REQUIRE('chlk.templates.controls.group_people_selector.UsersListTpl');

NAMESPACE('chlk.controls', function () {

    var selectors = {}, itemsCount = 30, filterTimeout, intervalNames = ['myStudentsInterval', 'allStudentsInterval', 'groupsInterval'];

    /** @class chlk.controls.GroupPeopleSelectorControl */
    CLASS(
        'GroupPeopleSelectorControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/group-people-selector/selector.jade')(this);
            },

            Boolean, 'hasAccessToLE',

            [ria.mvc.DomEventBind('click', '.group-people-selector .top-link:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function topLinkClick(node, event) {
                var parent = node.parent('.group-people-selector'),
                    selectorId = parent.getAttr('id'),
                    o = selectors[selectorId];
                parent.find('.top-link.pressed').removeClass('pressed');
                parent.find('.body-content.active').removeClass('active');
                node.addClass('pressed');
                parent.find('.body-content[data-index=' + node.getData('index') + ']').addClass('active');
                o.activityDom.addClass('partial-update');
                setTimeout(function(){
                    o.activityDom.removeClass('partial-update');
                }, 100);
            },

            [ria.mvc.DomEventBind('change', '.school-id, .grade-level-id')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function schoolGradeChange(node, event, selected_){
                var parent = node.parent('.body-content'),
                    selectorId = node.parent('.group-people-selector').getAttr('id'),
                    o = selectors[selectorId],
                    type = parent.getData('index'),
                    selector = node.parent('.group-people-selector'),
                    schoolId = parent.find('.school-id').getValue(),
                    gradeLevelId = parent.find('.grade-level-id').getValue(),
                    isMy = type == 1, studentsObj = isMy ? o.myStudentsPart : o.allStudentsPart;

                if(!schoolId){
                    studentsObj.classes = [];
                    this.updateStudents_(selector, type);
                }else{
                    var serviceIns = this.getContext().getService(chlk.services.ClassService),
                        ref = ria.reflection.ReflectionClass(chlk.services.ClassService),
                        methodRef = ref.getMethodReflector('getClassesBySchool'),
                        params = [new chlk.models.id.SchoolId(schoolId), new chlk.models.id.GradeLevelId(gradeLevelId)];

                    o.activityDom.addClass('partial-update');

                    methodRef.invokeOn(serviceIns, params)
                        .then(function(classes){
                            studentsObj.classes = classes || [];
                            this.updateStudents_(selector, type);
                        }.bind(this));
                }
            },

            function filterResultsByNode_(node){
                var type = node.parent('.body-content').getData('index'),
                    selector = node.parent('.group-people-selector');
                if(type == 3)
                    this.updateGroups_(selector);
                else
                    this.updateStudents_(selector, type);
            },

            [ria.mvc.DomEventBind('keyup change', '.top-filter')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function filterKeyup(node, event, selected_){
                clearTimeout(filterTimeout);
                var time = event.type == 'change' ? 1 : 1000, that = this;

                filterTimeout = setTimeout(function(){
                    var val = node.getValue();
                    if(val != node.getData('value')){
                        that.filterResultsByNode_(node);
                        node.setData('value', val);
                    }
                }, time);
            },

            [ria.mvc.DomEventBind('change', '.submit-on-change')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function filterChange(node, event, selected_){
                this.filterResultsByNode_(node);
            },

            function updateStudents_(selector, type, append_, start_){
                var selectorId = selector.getAttr('id'),
                    o = selectors[selectorId], isMy = type == 1, studentsObj = isMy ? o.myStudentsPart : o.allStudentsPart,
                    studentsData = o.students || [],
                    studentIds = studentsData.map(function(student){return student.getId()}),
                    intervalName = intervalNames[type];

                var parent = selector.find('.body-content[data-index=' + type + ']'),
                    dom = append_ ? parent.find('.items-cnt-2') : parent;
                var serviceIns = this.getContext().getService(chlk.services.StudentService);
                var ref = ria.reflection.ReflectionClass(chlk.services.StudentService);
                var methodRef = ref.getMethodReflector('getStudents'),
                    gradeLevelId = chlk.models.id.GradeLevelId(parent.find('.grade-level-id').getValue()),
                    schoolId = chlk.models.id.SchoolId(parent.find('.school-id').getValue()),
                    programId = chlk.models.id.ProgramId(parent.find('.program-id').getValue()),
                    classId = chlk.models.id.ClassId(parent.find('.class-id').getValue()),
                    byLastName = !!parseInt(parent.find('.by-last-name').getValue(),10),
                    filter = parent.find('.top-filter').getValue();
                var params = [classId, filter, isMy, byLastName, start_ || 0, itemsCount, null, schoolId, gradeLevelId, programId];
                var tpl = append_ ? new chlk.templates.controls.group_people_selector.PersonItemsTpl() :
                    new chlk.templates.controls.group_people_selector.UsersListTpl();
                tpl.options({
                    selected: studentIds,
                    userRole: this.context.getSession().get(ChlkSessionConstants.USER_ROLE),
                    hasAccessToLE: this.isHasAccessToLE(),
                    selectorMode: o.selectorMode
                });

                parent.addClass('scroll-freezed');

                if(!append_){
                    clearInterval(o[intervalName]);
                    o[intervalName] = null;
                    this.startInterval_(o, selector, type);
                    o.activityDom.addClass('partial-update');
                }

                methodRef.invokeOn(serviceIns, params)
                    .then(function(students){
                        if(students.getItems().length < itemsCount)
                            clearInterval(o[intervalName]);
                        var model = append_ ? students : new chlk.models.recipients.UsersListViewData(students, studentsObj.gradeLevels,
                            studentsObj.schools, studentsObj.programs, studentsObj.classes, gradeLevelId, schoolId, programId, classId, byLastName, filter);
                        tpl.assign(model);
                        if(!append_)
                            dom.empty();
                        tpl.renderTo(dom);
                        o.activityDom.removeClass('partial-update');
                        this.context.getDefaultView().notifyControlRefreshed();
                        parent.removeClass('scroll-freezed');
                    }, this);
            },

            function updateGroups_(selector, append_){
                var selectorId = selector.getAttr('id'), type = 3,
                    o = selectors[selectorId],
                    groupsData = o.groups || [],
                    groupIds = groupsData.map(function(student){return student.getId()}),
                    intervalName = intervalNames[type];

                var parent = selector.find('.body-content[data-index=' + type + ']'),
                    dom = append_ ? parent.find('.items-cnt-2') : parent;
                var serviceIns = this.getContext().getService(chlk.services.GroupService);
                var ref = ria.reflection.ReflectionClass(chlk.services.GroupService);
                var methodRef = ref.getMethodReflector('list'),
                    filter = parent.find('.top-filter').getValue();
                var params = [filter];
                var tpl = append_ ? new chlk.templates.controls.group_people_selector.GroupsListItemsTpl() :
                    new chlk.templates.controls.group_people_selector.GroupsListTpl();
                tpl.options({
                    selected: groupIds,
                    userRole: this.context.getSession().get(ChlkSessionConstants.USER_ROLE),
                    selectorMode: o.selectorMode
                });

                parent.addClass('scroll-freezed');

                if(!append_){
                    clearInterval(o[intervalName]);
                    o[intervalName] = null;
                    this.startInterval_(o, selector, type);
                    o.activityDom.addClass('partial-update');
                }

                methodRef.invokeOn(serviceIns, params)
                    .then(function(groups){
                        if(groups.length < itemsCount)
                            clearInterval(o[intervalName]);
                        var model = new chlk.models.recipients.GroupsListViewData(groups, filter);
                        tpl.assign(model);
                        if(!append_)
                            dom.empty();
                        tpl.renderTo(dom);
                        o.activityDom.removeClass('partial-update');
                        this.context.getDefaultView().notifyControlRefreshed();
                        parent.removeClass('scroll-freezed');
                    }, this);
            },

            [ria.mvc.DomEventBind('change', '.all-groups-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function allGroupsSelect(node, event, selected_){
                this.updateSelectedGroupsByNodes_(node.parent('.body-content').find('.recipient-check'), node.is(':checked'));
            },

            [ria.mvc.DomEventBind('change', '.all-persons-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function allPersonsSelect(node, event, selected_){
                this.updateSelectedStudentsByNodes_(node.parent('.body-content').find('.recipient-check'), node.is(':checked'));
            },

            [ria.mvc.DomEventBind('change', '.group-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function groupSelect(node, event, selected_){
                this.updateSelectedGroupsByNodes_(node, node.is(':checked'));
            },

            [ria.mvc.DomEventBind('change', '.student-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function studentSelect(node, event, selected_){
                this.updateSelectedStudentsByNodes_(node, node.is(':checked'));
            },

            [ria.mvc.DomEventBind('click', '.remove-group')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function groupRemove(node, event){
                this.updateSelectedGroupsByNodes_(node, false);
            },

            [ria.mvc.DomEventBind('click', '.remove-student')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentRemove(node, event){
                this.updateSelectedStudentsByNodes_(node, false);
            },

            function updateSelected_(selector, o){
                var model = new chlk.models.recipients.GroupSelectorViewData(null, null, null, null, null, null, null,
                    o.groups, o.students), selectedCount = o.groups.length + o.students.length,
                    selectedText = 'Selected ' + (selectedCount ? '(' + selectedCount + ')' : '');
                var cnt = selector.find('.selected-content'), tpl = new chlk.templates.controls.group_people_selector.SelectorBaseTpl();
                selector.find('.selected-link').setHTML(selectedText);

                tpl.assign(model);
                tpl.renderTo(cnt.empty());
            },

            function updateSelectedGroupsByNodes_(nodes, add_) {
                var selector = nodes.parent('.group-people-selector'),
                    selectorId = selector.getAttr('id'),
                    o = selectors[selectorId], id, name, index, check,
                    groupsData = o.groups || [],
                    groupIds = groupsData.map(function(group){return group.getId().valueOf()});

                if(add_){
                    nodes.forEach(function(node){
                        id = parseInt(node.getData('id'), 10);
                        if(!id)
                            id = node.getData('id');
                        if(groupIds.indexOf(id) == -1){
                            id = new chlk.models.id.GroupId(id);
                            name = node.getData('name').toString();
                            groupsData.push(new chlk.models.group.Group(name, id));
                            check = selector.find('.group-check[data-id=' + id + ']:not(:checked)');
                            check.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [true]);
                        }
                    });
                }else{
                    nodes.forEach(function(node){
                        id = parseInt(node.getData('id'), 10);
                        if(!id)
                            id = node.getData('id');
                        index = groupIds.indexOf(id);
                        if(index > -1){
                            check = selector.find('.group-check[data-id=' + id + ']:checked');
                            check.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
                            groupsData.splice(index, 1);
                            groupIds.splice(index, 1);
                        }
                    });
                }

                o.groups = groupsData;
                this.updateSelected_(selector, o);
            },

            function updateSelectedStudentsByNodes_(nodes, add_) {
                var selector = nodes.parent('.group-people-selector'),
                    selectorId = selector.getAttr('id'),
                    o = selectors[selectorId], id, name, gender, index, check,
                    studentsData = o.students || [],
                    studentIds = studentsData.map(function(student){return student.getId().valueOf()});

                if(add_){
                    nodes.forEach(function(node){
                        id = parseInt(node.getData('id'), 10);
                        if(!id)
                            id = node.getData('id');
                        if(studentIds.indexOf(id) == -1){
                            id = new chlk.models.id.SchoolPersonId(id);
                            name = node.getData('name').toString();
                            gender = node.getData('gender');
                            studentsData.push(new chlk.models.people.ShortUserInfo(null, null, id, name, gender));
                            check = selector.find('.student-check[data-id=' + id + ']:not(:checked)');
                            check.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [true]);
                        }
                    });
                }else{
                    nodes.forEach(function(node){
                        id = parseInt(node.getData('id'), 10);
                        if(!id)
                            id = node.getData('id');
                        index = studentIds.indexOf(id);
                        if(index > -1){
                            check = selector.find('.student-check[data-id=' + id + ']:checked');
                            check.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
                            studentsData.splice(index, 1);
                            studentIds.splice(index, 1);
                        }
                    });
                }

                o.students = studentsData;
                this.updateSelected_(selector, o);
            },

            Object, function prepare(data, attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.setHasAccessToLE(data.isHasAccessToLE());
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var myStudentsPart = model.getMyStudentsPart();
                        selectors[attributes.id] = {
                            selectorMode: model.getSelectorMode(),
                            groups: model.getSelectedGroups(),
                            students: model.getSelectedStudents(),
                            myStudentsPart: {
                                gradeLevels: myStudentsPart.getGradeLevels(),
                                schools: myStudentsPart.getSchools(),
                                programs: myStudentsPart.getPrograms(),
                                classes: myStudentsPart.getClasses()
                            },
                            allStudentsPart: {
                                gradeLevels: myStudentsPart.getGradeLevels(),
                                schools: myStudentsPart.getSchools(),
                                programs: myStudentsPart.getPrograms(),
                                classes: myStudentsPart.getClasses()
                            },
                            activityDom: activity.getDom()
                        };
                        var selector= ria.dom.Dom('#' + attributes.id),
                            o = selectors[attributes.id];
                        this.startInterval_(o, selector, 1);
                        this.startInterval_(o, selector, 2);
                        selector.addClass(model.getSelectorCSSClass());
                        //this.startInterval_(o, selector, 3);
                    }.bind(this));
                return attributes;
            },

            function startInterval_(o, selector, type){
                var parent = selector.find('.body-content[data-index=' + type + ']'),
                    intervalName = intervalNames[type],
                    isPage = selector.parent('#main').exists();

                o[intervalName] = setInterval(function(){
                    if(o[intervalName] && parent.is(':visible') && !parent.hasClass('scroll-freezed')){
                        var zoom = parseFloat(ria.dom.Dom('html').getCss('zoom')) || 1,
                            docHeight = (isPage ? ria.dom.Dom() : parent.find('.items-cnt-2')).height() * zoom,
                            toBottom = 500 * zoom,
                            docParent = isPage ? $(window) : parent.find('.items-cnt');

                        if(docParent.scrollTop() > docHeight - docParent.height() - toBottom){
                            var currentStart = parseInt(parent.find('.start-value').getValue(), 10) + itemsCount;
                            parent.find('.start-value').setValue(currentStart);
                            this.updateStudents_(selector, type, true, currentStart);
                        }
                    }
                }.bind(this), 250);
            }
        ]);
});