REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.recipients.PeoplePageTpl');
REQUIRE('chlk.templates.controls.group_people_selector.GroupsListItemsTpl');

NAMESPACE('chlk.activities.recipients', function(){

    /**@class chlk.activities.recipients.PeoplePage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.recipients.PeoplePageTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.controls.group_people_selector.GroupsListItemsTpl, '', '.groups-cnt .items-cnt-2', ria.mvc.PartialUpdateRuleActions.Append)],
        'PeoplePage', EXTENDS(chlk.activities.lib.TemplatePage),[

        ]);
});