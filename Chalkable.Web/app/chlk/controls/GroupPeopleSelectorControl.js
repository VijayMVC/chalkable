REQUIRE('chlk.controls.Base');
REQUIRE('chlk.templates.controls.GroupsListItemsTpl');
REQUIRE('chlk.templates.controls.GroupsListTpl');
REQUIRE('chlk.templates.controls.PersonItemsTpl');
REQUIRE('chlk.templates.controls.UsersListTpl');

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

            Object, function prepare(model, attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {

                    }.bind(this));
                return attributes;
            }
        ]);
});