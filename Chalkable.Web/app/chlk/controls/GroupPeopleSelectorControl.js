REQUIRE('chlk.controls.Base');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListItemsTpl');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListTpl');
REQUIRE('chlk.templates.controls.group_people_selector.PersonItemsTpl');
REQUIRE('chlk.templates.controls.group_people_selector.UsersListTpl');

NAMESPACE('chlk.controls', function () {

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

            [ria.mvc.DomEventBind('change', '.all-items-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function allItemsSelect(node, event, selected_){
                node.parent('.body-content').find('.recipient-check').forEach(function(element){
                    element.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [node.is(':checked')]);
                });

                this.updateSelectedItems_(node.parent('.group-people-selector'));
            },

            [ria.mvc.DomEventBind('change', '.recipient-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function recipientSelect(node, event, selected_){
                this.updateSelectedItems_(node.parent('.group-people-selector'));
            },

            function updateSelectedItems_(selector){
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

            },

            Object, function prepare(data, attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        //ria.dom.Dom('#' + attributes.id).setData('model', data)
                    }.bind(this));
                return attributes;
            }
        ]);
});