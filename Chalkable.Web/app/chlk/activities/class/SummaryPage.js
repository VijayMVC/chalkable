REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.class.ClassSummary');
REQUIRE('chlk.templates.announcement.AnnouncementsByDate');

NAMESPACE('chlk.activities.class', function () {

    /** @class chlk.activities.class.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.class.ClassSummary)],
        'SummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});