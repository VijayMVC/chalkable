REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassSummary');
REQUIRE('chlk.templates.announcement.AnnouncementsByDate');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassSummary)],
        'SummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});