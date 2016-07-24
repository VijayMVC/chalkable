REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.UpgradeDistrictsListTpl');
REQUIRE('chlk.templates.school.UpgradeSchoolsListTpl');


NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.UpgradeDistrictsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.UpgradeDistrictsListTpl)],
        //[ria.mvc.PartialUpdateRule(chlk.templates.school.DistrictForUpgradeTpl), '', '.district-info', ria.mvc.PartialUpdateRuleActions.Replace],
        'UpgradeDistrictsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.upgrade-district-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function upgradeDistrictClick(node, event){
                jQuery(node.parent().find('.upgrade-district-date-picker')).select();
            },

            [ria.mvc.DomEventBind('click', '.upgrade-school-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function upgradeSchoolClick(node, event){
                jQuery(node.parent().find('.upgrade-school-date-picker')).select();
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.districts-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number, Boolean]],
            function selectDistrict(node, event, row, index, noScroll_){
                if(row.hasClass('school-info'))  return false;

                clearTimeout(this._slideTimeout);
                this._slideTimeout = setTimeout(function(){
                    node.find('.schools-list-container:eq(' + index + ')').slideDown(500);
                    row.find('.triangle').addClass('down');
                    jQuery(row.find('.show-schools-link').valueOf()).click();
                }.bind(this), 500);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '.districts-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function deSelectDistrict(node, event, row, index){
                node.find('.schools-list-container:eq(' + index + ')').slideUp(250);
                row.find('.triangle').removeClass('down');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.school.UpgradeSchoolsListTpl)],
            [[Object, Object, String]],
            VOID, function updateSchoolsList(tpl, model, msg_) {

                var districtId = model.getDistrictId();
                var node = this.dom.find('.schools-list-container[data-districtId='+ districtId.valueOf() + ']').empty();
                tpl.renderTo(node);
            }
        ]);
});