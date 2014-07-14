REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.AddDuplicateAnnouncementTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddDuplicateAnnouncementDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
//        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddDuplicateAnnouncementTpl)],
        'AddDuplicateAnnouncementDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            chlk.models.id.ClassId, 'originalClassId',

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setOriginalClassId(model.getSelectedClassId());
            },

            [ria.mvc.DomEventBind('click', '.class-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function classClick(node, event){
//                if(!this.dom.find('.is-edit').getData('isedit')){
                var originalClassId = this.getOriginalClassId();
                if(originalClassId && node.getAttr('classId') == originalClassId.valueOf()) return;

                node.toggleClass('pressed');
                var classIds = [];
                this.dom.find('.class-button.pressed').forEach(function(item){
                    if(originalClassId && item.getAttr('classId') != originalClassId.valueOf())
                        classIds.push(item.getAttr('classId'));
                });
                this.dom.find('input[name=selectedIds]').setValue(classIds.join(','));
                var sBt = this.dom.find('.add-duplicate-btn');
                if(classIds.length > 0){
                    sBt.removeClass('disabled');
                    sBt.removeAttr('disabled');
                    sBt.find('button').removeAttr('disabled');
                }else{
                    sBt.find('button').setAttr('disabled', 'disabled');
                    sBt.setAttr('disabled');
                    sBt.addClass('disabled');
                }
            }
        ]);
});