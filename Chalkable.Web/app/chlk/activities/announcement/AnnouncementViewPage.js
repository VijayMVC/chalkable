REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.StudentAnnouncement');
REQUIRE('chlk.templates.class.TopBar');
REQUIRE('chlk.models.grading.Mapping');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.AnnouncementViewPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView)],
        'AnnouncementViewPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            ria.dom.Dom, 'currentContainer',

            chlk.models.grading.Mapping, 'mapping',
            Array, 'applicationsInGradeView',

            [ria.mvc.DomEventBind('keypress', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputClick(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    var value = node.getValue();
                    value = parseInt(value, 10);
                    if(value > 100)
                        value = 100;
                    if(value < 0)
                        value = 0;
                    node.setValue(value);
                    this.updateItem(node);
                }
            },

            [[ria.dom.Dom]],
            VOID, function updateItem(node){
                var row = node.parent('.row');
                this.setCurrentContainer(row.find('.top-content'));
                var form = row.find('form');
                form.triggerEvent('submit');
                /*var o = form.serialize();
                var serializer = new ria.serialize.JsonSerializer();
                var model = serializer.deserialize(o, chlk.models.announcement.StudentAnnouncement);*/
                var next = row.next();
                if(next.exists()){
                    row.removeClass('selected');
                    next.addClass('selected');
                }
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setMapping(model.getStudentAnnouncements().getMapping());
                this.setApplicationsInGradeView(model.getApplications().filter(function(item){return item.applicationviewdata.showingradeview}));
                var that = this;
                jQuery(this.dom.valueOf()).on('change', '.grade-select', function(){
                    var node = new ria.dom.Dom(this);
                    var row = node.parent('.row');
                    row.find('.grade-input').setValue(node.getValue());
                    that.updateItem(node);
                });
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.StudentAnnouncement)],
            VOID, function doUpdateItem(tpl, model, msg_) {
                tpl.options({
                    gradingMapping: this.getMapping(),
                    applicationsInGradeView: this.getApplicationsInGradeView(),
                    ownerPictureUrl: this.dom.find('[name=ownerPictureUrl]').getValue(),
                    notAnnouncement: !!this.dom.find('[name=notAnnouncement]').getValue(),
                    readonly: !!this.dom.find('[name=readonly]').getValue(),
                    gradingStyle: parseInt(this.dom.find('[name=gradingStyle]').getValue(), 10)
                });
                this.getCurrentContainer().empty();
                tpl.renderTo(this.getCurrentContainer());
            }
        ]
    );
});