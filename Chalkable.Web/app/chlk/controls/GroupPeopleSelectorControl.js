REQUIRE('chlk.controls.Base');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListItemsTpl');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListTpl');
REQUIRE('chlk.templates.controls.group_people_selector.PersonItemsTpl');
REQUIRE('chlk.templates.controls.group_people_selector.UsersListTpl');

NAMESPACE('chlk.controls', function () {

    var selectors = {};

    /** @class chlk.controls.GroupPeopleSelectorControl */
    CLASS(
        'GroupPeopleSelectorControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/group-people-selector/selector.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.group-people-selector .top-link:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function topLinkClick(node, event) {
                var parent = node.parent('.group-people-selector');
                parent.find('.top-link.pressed').removeClass('pressed');
                parent.find('.body-content.active').removeClass('active');
                node.addClass('pressed');
                parent.find('.body-content[data-index=' + node.getData('index') + ']').addClass('active');
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

            /*function updateSelectedItems_(selector){
                var processedStudents = {}, id, name, gender, selectedStudents = [],
                    selectedGroups = [], studentIds = [], groupIds = [];
                selector.find('.student-check:checked').forEach(function(node){
                    id = parseInt(node.getData('id'), 10);
                    if(!id)
                        id = node.getData('id');
                    if(!processedStudents[id]){
                        studentIds.push(id);
                        processedStudents[id] = true;
                        id = new chlk.models.id.SchoolPersonId(id);
                        name = node.getData('name');
                        gender = node.getData('gender');
                        selectedStudents.push(new chlk.models.people.ShortUserInfo(null, null, id, name, gender));
                    }
                });

                selector.find('.group-check:checked').forEach(function(node){
                    id = parseInt(node.getData('id'), 10);
                    if(!id)
                        id = node.getData('id');
                    groupIds.push(id);
                    id = new chlk.models.id.GroupId(id);
                    name = node.getData('name').toString();
                    selectedGroups.push(new chlk.models.group.Group(name, id));
                });

                var model = new chlk.models.recipients.GroupSelectorViewData(null, null, null, null, null, null, null,
                    selectedGroups, selectedStudents);
                var cnt = selector.find('.selected-content'), tpl = new chlk.templates.controls.group_people_selector.SelectorBaseTpl();

                tpl.assign(model);
                tpl.renderTo(cnt.empty());

            },*/

            function updateSelected_(selector, o){
                var model = new chlk.models.recipients.GroupSelectorViewData(null, null, null, null, null, null, null,
                    o.groups, o.students);
                var cnt = selector.find('.selected-content'), tpl = new chlk.templates.controls.group_people_selector.SelectorBaseTpl();

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
                            check = selector.find('.group-check[data-id=' + id + ']:checked');
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
                            check = selector.find('.group-check[data-id=' + id + ']:not(:checked)');
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
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        selectors[attributes.id] = {
                            groups: model.getSelectedGroups(),
                            students: model.getSelectedStudents()
                        };
                        //ria.dom.Dom('#' + attributes.id).setData('model', data)
                    }.bind(this));
                return attributes;
            }
        ]);
});